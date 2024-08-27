using MediatR;
using System.Threading;
using System.Threading.Tasks;
using IdpServer.Application.Service.Abstraction;
using IdpServer.Application.Client.Model;

namespace IdpServer.Application.Client.Command.Handler
{
    public class GenerateSecretClientHandler : IRequestHandler<GenerateClientSecret, SecretClientDto>
    {
        private readonly IClientService _service;
        public GenerateSecretClientHandler(IClientService service)
        {
            _service = service;
        }

        public Task<SecretClientDto> Handle(GenerateClientSecret request, CancellationToken cancellationToken)
        {
            return _service.GenerateClientSecretAsync(request, cancellationToken);
        }
    }
}
