using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AHI.Infrastructure.Service;
using IdpServer.Application.Repository.Abstraction;
using IdpServer.Application.Service.Abstraction;
using IdpServer.Application.User.Command;
using IdpServer.Application.User.Model;

namespace IdpServer.Application.Service
{
    public class LoginTypeService : BaseSearchService<Domain.Entity.LoginType, string, SearchLoginType, LoginTypeDto>, ILoginTypeService
    {
        private readonly ILoginTypeRepository _loginTypeRepository;

        public LoginTypeService(IServiceProvider serviceProvider, ILoginTypeRepository loginTypeRepository)
            : base(LoginTypeDto.Create, serviceProvider)
        {
            _loginTypeRepository = loginTypeRepository;
        }

        protected override Type GetDbType()
        {
            return typeof(ILoginTypeRepository);
        }

        public Task<List<LoginTypeDto>> GetLoginTypesAsync()
        {
            return _loginTypeRepository.GetLoginTypesAsync();
        }
    }
}
