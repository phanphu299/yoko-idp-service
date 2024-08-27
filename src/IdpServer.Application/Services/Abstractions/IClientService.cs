using AHI.Infrastructure.Service.Abstraction;
using System.Threading.Tasks;
using System.Threading;
using IdpServer.Application.SharedKernel;
using IdpServer.Application.Client.Command;
using IdpServer.Application.Client.Model;
using System.Collections.Generic;

namespace IdpServer.Application.Service.Abstraction
{
    public interface IClientService : ISearchService<Domain.Entity.Client, int, SearchClient, ClientDto>
    {
        Task<ClientDto> AddClientAsync(AddClient model, CancellationToken token);
        Task<BaseResponse> UpdateClientAsync(UpdateClient model, CancellationToken token);
        Task<BaseResponse> DeleteClientAsync(DeleteClient command, CancellationToken token);
        Task<ClientInformationDto> GetClientByClientIdAsync(GetClientById command, CancellationToken token);
        Task<SecretClientDto> GenerateClientSecretAsync(GenerateClientSecret command, CancellationToken token);
        Task<BaseResponse> CheckExistClientsAsync(CheckExistClient command, CancellationToken cancellationToken);
        Task<ClientDto> FetchByClientIdAsync(string clientId);
        Task PartialUpdateAsync(PartialUpdateClient command, CancellationToken cancellationToken);
        Task<IEnumerable<ArchiveClientDto>> ArchiveAsync(ArchiveClient command, CancellationToken token);
        Task<AHI.Infrastructure.SharedKernel.Model.BaseResponse> RetrieveAsync(RetrieveClient command, CancellationToken token);
        Task<AHI.Infrastructure.SharedKernel.Model.BaseResponse> VerifyArchiveDataAsync(VerifyClient command, CancellationToken token);
    }
}
