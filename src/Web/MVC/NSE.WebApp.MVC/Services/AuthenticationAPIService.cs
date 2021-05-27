using Microsoft.Extensions.Options;
using NSE.WebApp.MVC.Extensions;
using NSE.WebApp.MVC.Models;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace NSE.WebApp.MVC.Services
{
    public class AuthenticationAPIService : Service, IAuthenticationAPIService
    {
        private readonly HttpClient _httpClient;

        public AuthenticationAPIService(HttpClient httpClient, 
                                        IOptions<AppSettings> settings)
        {
            httpClient.BaseAddress = new Uri(settings.Value.AuthenticationUrl);
            _httpClient = httpClient;
        }

        public async Task<UserResponseLogin> Login(UserLogin userLogin)
        {
            var loginContent = GetContent(userLogin);

            var uri = "/api/identity/authentication";
            var response = await _httpClient.PostAsync(uri, loginContent);

            

            if (!HandleResponseErrors(response))
            {
                return new UserResponseLogin
                {
                    ResponseResult = await DeserializeObjectResponse<ResponseResult>(response)
                };
            }

            return await DeserializeObjectResponse<UserResponseLogin>(response);
        }

        public async Task<UserResponseLogin> Register(UserRegister userRegister)
        {
            var recordContent = GetContent(userRegister);

            var uri = "/api/identity/new-account";
            var response = await _httpClient.PostAsync(uri, recordContent);

            if (!HandleResponseErrors(response))
            {
                return new UserResponseLogin
                {
                    ResponseResult = await DeserializeObjectResponse<ResponseResult>(response)
                };
            }

            return await DeserializeObjectResponse<UserResponseLogin>(response);
        }
    }
}
