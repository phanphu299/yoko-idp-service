using System;
using IdpServer.Application.Model;
using MediatR;

namespace IdpServer.Application.User.Command
{
    public class UpdateUserInfo : IRequest<UserDto>
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Avatar { get; set; }
        public string DateTimeFormat { get; set; }
        public string DisplayDateTimeFormat { get; set; }
        public int TimeZoneId { get; set; }
        public string LoginTypeCode { get; set; }
    }
}