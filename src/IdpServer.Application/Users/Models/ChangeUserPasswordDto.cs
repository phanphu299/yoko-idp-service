using AHI.Infrastructure.Exception;

namespace IdpServer.Application.User.Model
{
    public class ChangeUserPasswordDto
    {
        public bool Result { get; set; }
        public string Message { get; set; }
        public BaseException Exception { get; set; }

        public ChangeUserPasswordDto(bool result, string message, BaseException exception = null)
        {
            Result = result;
            Message = message;
            Exception = exception;
        }
    }
}