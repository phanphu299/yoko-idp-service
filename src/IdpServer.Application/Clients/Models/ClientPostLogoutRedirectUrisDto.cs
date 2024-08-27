using System;
using System.Linq.Expressions;
using Newtonsoft.Json;

namespace IdpServer.Application.Client.Model
{
    public class ClientPostLogoutRedirectUrisDto
    {
        [JsonIgnore]
        public int Id { get; set; }
        public string PostLogoutRedirectUri { get; set; }
        [JsonIgnore]
        public int ClientId { get; set; }
        public static Expression<Func<Domain.Entity.ClientPostLogoutRedirectUris, ClientPostLogoutRedirectUrisDto>> Projection
        {
            get
            {
                return model => new ClientPostLogoutRedirectUrisDto
                {
                    Id = model.Id,
                    PostLogoutRedirectUri = model.PostLogoutRedirectUri,
                    ClientId = model.ClientId
                };
            }
        }

        public static ClientPostLogoutRedirectUrisDto Create(Domain.Entity.ClientPostLogoutRedirectUris model)
        {
            if (model == null)
            {
                return null;
            }
            return Projection.Compile().Invoke(model);
        }
    }
}
