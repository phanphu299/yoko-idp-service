using AHI.Infrastructure.Service.Abstraction;
using System.Threading.Tasks;
using System.Threading;
using IdpServer.Application.SharedKernel;
using IdpServer.Application.BrokerClient.Command;
using IdpServer.Application.BrokerClient.Model;

namespace IdpServer.Application.Service.Abstraction
{
    public interface IBrokerClientService : ISearchService<Domain.Entity.BrokerClient, string, SearchBrokerClient, BrokerClientDto>
    {
        Task<BrokerClientDto> GetByIdAsync(string id, CancellationToken token);
        Task<BrokerClientDto> AddBrokerClientAsync(AddBrokerClient command, CancellationToken token);
        Task<BrokerClientDto> UpdateBrokerClientAsync(UpdateBrokerClient command, CancellationToken token);
        Task<BaseResponse> DeleteBrokerClientAsync(DeleteBrokerClient command, CancellationToken token);
        Task<BrokerClientDto> FetchByIdAsync(string id);
    }
}
