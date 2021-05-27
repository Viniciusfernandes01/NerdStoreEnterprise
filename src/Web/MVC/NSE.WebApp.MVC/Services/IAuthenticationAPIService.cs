using NSE.WebApp.MVC.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NSE.WebApp.MVC.Services
{
    public interface IAuthenticationAPIService
    {
        Task<UserResponseLogin> Login(UserLogin userLogin);
        Task<UserResponseLogin> Register(UserRegister userRegister);
    }
}
