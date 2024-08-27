using System;
using System.Linq.Expressions;

namespace IdpServer.Application.Client.Model
{
    public class SecretClientDto
    {
        public string ClientId { get; set; }
        public string ClientSecretRaw { get; set; }
        public DateTime Created { get; set; }

        public static Expression<Func<Domain.Entity.Client, SecretClientDto>> Projection
        {
            get
            {
                return entity => new SecretClientDto
                {
                    ClientId = entity.ClientId,
                    Created = entity.Created
                };
            }
        }
        public static SecretClientDto Create(Domain.Entity.Client entity)
        {
            if (entity == null)
                return null;
            return Projection.Compile().Invoke(entity);
        }
    }
}
