using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Function.Service.Abstraction;

namespace Function.Http
{
    public class PasswordReminderTimer
    {
        private readonly IUserService _userService;

        public PasswordReminderTimer(IUserService userService)
        {
            _userService = userService;
        }

        [FunctionName("PasswordChangeReminder")]
        public Task RunAsync([TimerTrigger("0 0 0 * * *")] TimerInfo timer)
        {
            return _userService.ScanPasswordExpirationAsync();
        }
    }
}