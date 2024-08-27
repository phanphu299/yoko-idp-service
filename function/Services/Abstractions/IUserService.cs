using System;
using System.Threading.Tasks;
using Function.Model;

namespace Function.Service.Abstraction
{
    public interface IUserService
    {
        Task ScanPasswordExpirationAsync();
    }
}