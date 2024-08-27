using MediatR;
using IdpServer.Application.Client.Model;

namespace IdpServer.Application.Client.Command
{
    public class FetchClientById : IRequest<ClientDto>
    {
        public string ClientId { get; set; }

        public FetchClientById(string clientId)
        {
            ClientId = clientId;
        }
    }
}
