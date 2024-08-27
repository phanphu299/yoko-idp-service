using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using AHI.Infrastructure.Service.Tag.Extension;
using AHI.Infrastructure.Service.Tag.Model;

namespace IdpServer.Application.Client.Model
{
    public class ClientDto : TagDtos
    {
        public int Id { get; set; }
        public string ClientName { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public DateTime Created { get; set; }
        public IEnumerable<ClientRedirectUrisDto> ClientRedirectUris { get; set; }
        public IEnumerable<ClientPostLogoutRedirectUrisDto> ClientPostLogoutRedirectUris { get; set; }
        public ClientDto()
        {
        }

        public static Expression<Func<Domain.Entity.Client, ClientDto>> Projection
        {
            get
            {
                return entity => new ClientDto
                {
                    Id = entity.Id,
                    ClientName = entity.ClientName,
                    ClientId = entity.ClientId,
                    Created = entity.Created,
                    ClientPostLogoutRedirectUris = entity.ClientPostLogoutRedirectUris.Select(ClientPostLogoutRedirectUrisDto.Create),
                    ClientRedirectUris = entity.ClientRedirectUris.Select(ClientRedirectUrisDto.Create),
                    Tags = entity.EntityTags.MappingTagDto()
                };
            }
        }

        public static ClientDto Create(Domain.Entity.Client entity)
        {
            if (entity == null)
                return null;
            return Projection.Compile().Invoke(entity);
        }
    }
}
