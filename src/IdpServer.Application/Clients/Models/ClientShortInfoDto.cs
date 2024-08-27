using System;
using System.Collections.Generic;

namespace IdpServer.Application.Client.Model
{
    public class ClientShortInfoDto
    {
        public Guid ClientId { get; set; }
        // public IEnumerable<string> RightHashes { get; set; }
        public IEnumerable<string> RightShorts { get; set; }
        public IEnumerable<string> ObjectRightShorts { get; set; }

        public ClientShortInfoDto()
        {
            RightShorts = new List<string>();
            ObjectRightShorts = new List<string>();
        }
    }
}
