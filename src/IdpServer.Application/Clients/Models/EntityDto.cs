using System.Collections.Generic;

namespace IdpServer.Application.Client.Model
{
    public class EntityDto
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public IEnumerable<PrivilegeDto> Privileges { get; set; }
        public EntityDto()
        {
            Privileges = new List<PrivilegeDto>();
        }
    }
}