using System;
using System.Linq.Expressions;
using Newtonsoft.Json;

namespace IdpServer.Application.Client.Model
{
    public class ClientRedirectUrisDto
    {
        [JsonIgnore]
        public int Id { get; set; }
        public string RedirectUri { get; set; }
        [JsonIgnore]
        public int ClientId { get; set; }

        public static Expression<Func<Domain.Entity.ClientRedirectUris, ClientRedirectUrisDto>> Projection
        {
            get
            {
                return model => new ClientRedirectUrisDto
                {
                    Id = model.Id,
                    RedirectUri = model.RedirectUri,
                    ClientId = model.ClientId
                };
            }
        }

        public static ClientRedirectUrisDto Create(Domain.Entity.ClientRedirectUris model)
        {
            if (model == null)
            {
                return null;
            }
            return Projection.Compile().Invoke(model);
        }
    }
}
