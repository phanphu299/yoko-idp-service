using MediatR;
using IdpServer.Application.Client.Model;
using System.Collections.Generic;
using System;

namespace IdpServer.Application.Client.Command
{
    public class ArchiveClient : IRequest<IEnumerable<ArchiveClientDto>>
    {
        public DateTime ArchiveTime { get; set; } = DateTime.UtcNow;
    }
}
