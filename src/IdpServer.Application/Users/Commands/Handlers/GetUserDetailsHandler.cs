using System.Threading;
using System.Threading.Tasks;
using AHI.Infrastructure.Exception;
using IdpServer.Application.Constant;
using IdpServer.Application.Model;
using IdpServer.Application.Service.Abstraction;
using MediatR;

namespace IdpServer.Application.User.Command
{
    public class GetUserDetailsHandler : IRequestHandler<GetUserDetails, UserDto>
    {
        private readonly IUserService _service;
        public GetUserDetailsHandler(IUserService service)
        {
            _service = service;
        }
        public async Task<UserDto> Handle(GetUserDetails request, CancellationToken cancellationToken)
        {
            var entity = (UserDto)null;
            if(!string.IsNullOrEmpty(request.Upn))
                entity = await _service.FindByUpnAsync(request.Upn, request.IgnoreQueryFilters);
            else
                entity = await _service.FindByIdAsync(request.UserId, request.IgnoreQueryFilters);
            if(entity == null)
                throw new EntityNotFoundException(MessageConstants.ENTITY_NOT_FOUND);
            return entity;
        }
    }
}
