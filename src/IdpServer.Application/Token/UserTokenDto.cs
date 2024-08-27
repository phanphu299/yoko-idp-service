using System;
using System.Linq.Expressions;

namespace IdpServer.Application.Model
{
    public class UserTokenDto
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string TokenKey { get; set; }
        public string TokenType { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ExpiredDate { get; set; }
        public string RedirectUrl { get; set; }
        public bool Deleted { get; set; }

        public static Expression<Func<Domain.Entity.UserToken, UserTokenDto>> Projection
        {
            get
            {
                return entity => new UserTokenDto
                {
                    Id = entity.Id,
                    UserName = entity.UserName,
                    TokenKey = entity.TokenKey,
                    TokenType = entity.TokenType,
                    CreatedDate = entity.CreatedDate,
                    ExpiredDate = entity.ExpiredDate,
                    RedirectUrl = entity.RedirectUrl,
                    Deleted = entity.Deleted
                };
            }
        }

        public static UserTokenDto Create(Domain.Entity.UserToken entity)
        {
            if (entity == null)
                return null;
            return Projection.Compile().Invoke(entity);
        }
    }
}