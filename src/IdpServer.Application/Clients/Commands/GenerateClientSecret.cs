using System;
using IdpServer.Application.Client.Model;
using MediatR;

namespace IdpServer.Application.Client.Command
{
    public class GenerateClientSecret : IRequest<SecretClientDto>
    {
        public Guid ClientId { get; set; }
    }
}
