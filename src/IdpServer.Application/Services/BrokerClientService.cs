using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AHI.Infrastructure.Cache.Abstraction;
using AHI.Infrastructure.Exception;
using AHI.Infrastructure.Service;
using AHI.Infrastructure.SharedKernel.Extension;
using IdpServer.Application.BrokerClient.Command;
using IdpServer.Application.BrokerClient.Model;
using IdpServer.Application.Repository.Abstraction;
using IdpServer.Application.Service.Abstraction;
using IdpServer.Application.SharedKernel;

namespace IdpServer.Application.Service
{
    public class BrokerClientService : BaseSearchService<Domain.Entity.BrokerClient, string, SearchBrokerClient, BrokerClientDto>, IBrokerClientService
    {
        private const int MINIMUM_PASSWORD_LENGTH = 10;
        private const int MAXIMUM_PASSWORD_LENGTH = 64;
        private const int DEFAULT_PASSWORD_LENGTH = 30;
        private readonly IIdpUnitOfWork _unitOfWork;
        private readonly ICache _cache;

        public BrokerClientService(
             IServiceProvider serviceProvider,
             IIdpUnitOfWork unitOfWork,
             ICache cache)
            : base(BrokerClientDto.Create, serviceProvider)
        {
            _unitOfWork = unitOfWork;
            _cache = cache;
        }

        protected override Type GetDbType()
        {
            return typeof(IBrokerClientRepository);
        }

        private string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            Random rand = new Random();
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[rand.Next(s.Length)]).ToArray());
        }

        private async Task<string> GenerateClientId(string clientId = "")
        {
            if (string.IsNullOrEmpty(clientId))
            {
                clientId = RandomString(16);
            }
            var checkEntity = await _unitOfWork.BrokerClients.FindAsync(clientId);
            if (checkEntity == null)
            {
                return clientId;
            }
            clientId = RandomString(16);
            return await GenerateClientId(clientId);
        }


        public async Task<BrokerClientDto> AddBrokerClientAsync(AddBrokerClient command, CancellationToken token)
        {
            if (command.PasswordLength < MINIMUM_PASSWORD_LENGTH || command.PasswordLength > MAXIMUM_PASSWORD_LENGTH)
            {
                command.PasswordLength = DEFAULT_PASSWORD_LENGTH;
            }

            var clientId = await GenerateClientId();
            var entity = new Domain.Entity.BrokerClient()
            {
                Id = clientId,
                Password = RandomString(command.PasswordLength),
                CreatedBy = command.CreatedBy,
                ExpiredDays = command.ExpiredDays,
                TenantId = command.TenantId,
                SubscriptionId = command.SubscriptionId,
                ProjectId = command.ProjectId,
                CreatedUtc = DateTime.UtcNow,
                UpdatedUtc = DateTime.UtcNow,
                ExpiredUtc = DateTime.UtcNow.AddDays(command.ExpiredDays)
            };
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var addedClient = await _unitOfWork.BrokerClients.AddAsync(entity);
                await _unitOfWork.CommitAsync();
                var result = BrokerClientDto.Create(addedClient);
                return result;
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }

        public async Task<BrokerClientDto> UpdateBrokerClientAsync(UpdateBrokerClient command, CancellationToken token)
        {
            if (command.PasswordLength < MINIMUM_PASSWORD_LENGTH || command.PasswordLength > MAXIMUM_PASSWORD_LENGTH)
            {
                command.PasswordLength = DEFAULT_PASSWORD_LENGTH;
            }

            var client = await _unitOfWork.BrokerClients.FindAsync(command.Id);
            if (client == null)
                throw new EntityNotFoundException();

            client.Password = RandomString(command.PasswordLength);
            client.CreatedBy = command.UpdatedBy;
            if (command.ExpiredDays.HasValue)
            {
                client.ExpiredDays = command.ExpiredDays.Value;
                client.ExpiredUtc = DateTime.UtcNow.AddDays(command.ExpiredDays.Value);
            }

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var result = await _unitOfWork.BrokerClients.UpdateAsync(command.Id, client);
                await _unitOfWork.CommitAsync();

                var key = $"idp_broker_client_{client.Id.CalculateMd5Hash()}_*";
                await _cache.DeleteAllKeysAsync(key);
                return BrokerClientDto.Create(result);
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }

        public async Task<BaseResponse> DeleteBrokerClientAsync(DeleteBrokerClient command, CancellationToken token)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var clientIds = command.Ids.Distinct();
                if (clientIds == null || clientIds.Any() == false)
                {
                    return BaseResponse.Failed;
                }

                var tasks = new List<Task>();
                foreach (var id in clientIds)
                {
                    var client = await _unitOfWork.BrokerClients.FindAsync(id);
                    if (client == null)
                        throw new EntityNotFoundException(detailCode: ExceptionErrorCode.DetailCode.ERROR_ENTITY_NOT_FOUND_SOME_ITEMS_DELETED);
                    var key = $"idp_broker_client_{client.Id.CalculateMd5Hash()}_*";
                    await _unitOfWork.BrokerClients.RemoveAsync(client.Id);
                    tasks.Add(_cache.DeleteAllKeysAsync(key));
                }
                await _unitOfWork.CommitAsync();

                await Task.WhenAll(tasks);
                return BaseResponse.Success;
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }

        public async Task<BrokerClientDto> FetchByIdAsync(string id)
        {
            var entity = await _unitOfWork.BrokerClients.AsFetchable().Where(x => x.Id == id).FirstOrDefaultAsync();
            return BrokerClientDto.Create(entity);
        }

        public async Task<BrokerClientDto> GetByIdAsync(string id, CancellationToken token)
        {
            var client = await _unitOfWork.BrokerClients.FindAsync(id);
            if (client == null)
                throw new EntityNotFoundException();
            return BrokerClientDto.Create(client);
        }
    }
}
