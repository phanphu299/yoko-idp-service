using System;
using System.Linq.Expressions;

namespace IdpServer.Application.User.Model
{
    public class LoginTypeDto
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public static Expression<Func<Domain.Entity.LoginType, LoginTypeDto>> Projection
        {
            get
            {
                return model => new LoginTypeDto
                {
                    Code = model.Id,
                    Name = model.Name
                };
            }
        }

        public static LoginTypeDto Create(Domain.Entity.LoginType model)
        {
            if (model == null)
            {
                return null;
            }
            return Projection.Compile().Invoke(model);
        }
    }
}
