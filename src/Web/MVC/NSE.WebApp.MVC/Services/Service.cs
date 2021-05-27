using NSE.WebApp.MVC.Extensions;
using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace NSE.WebApp.MVC.Services
{
    public abstract class Service
    {
        protected StringContent GetContent(object data)
        {
            return new StringContent(
                JsonSerializer.Serialize(data),
                Encoding.UTF8,
                "application/json");
        }

        protected async Task<T> DeserializeObjectResponse<T>(HttpResponseMessage responseMessage)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            };

            return JsonSerializer.Deserialize<T>(await responseMessage.Content.ReadAsStringAsync(), options);
        }


        protected bool HandleResponseErrors(HttpResponseMessage response)
        {
            if ((int)response.StatusCode == 400) return false;

            if (!response.IsSuccessStatusCode)
            {
                throw new CustomHttpRequestException(response.StatusCode);
            }

            return true;
        }
    }
}
