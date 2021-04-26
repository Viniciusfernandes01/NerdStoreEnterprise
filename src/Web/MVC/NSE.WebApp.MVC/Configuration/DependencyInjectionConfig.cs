using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NSE.WebApp.MVC.Extensions;
using NSE.WebApp.MVC.Services;
using NSE.WebApp.MVC.Services.Handlers;
using Polly;
using Polly.Extensions.Http;
using Polly.Retry;
using System;
using System.Net.Http;

namespace NSE.WebApp.MVC.Configuration
{
    public static class DependencyInjectionConfig
    {
        public static void RegisterServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<HttpClientAuthorizationDelegatingHandler>();

            services.AddHttpClient<IAuthenticationAPIService, AuthenticationAPIService>();

            var retryPolicy = HttpPolicyExtensions
                .HandleTransientHttpError()
                .WaitAndRetryAsync(new[]
                {
                    TimeSpan.FromSeconds(1),
                    TimeSpan.FromSeconds(5),
                    TimeSpan.FromSeconds(10)
                }, (outcome, timespan, retryCount, context) =>
                {
                    Console.WriteLine($"Retry for the {retryCount} time");
                });

            services.AddHttpClient<ICatalogService, CatalogService>()
                .AddHttpMessageHandler<HttpClientAuthorizationDelegatingHandler>()
                //.AddTransientHttpErrorPolicy(
                //    p => p.WaitAndRetryAsync(3, _ => TimeSpan.FromMilliseconds(600)));

                //services.AddHttpClient("Refit", 
                //        options => 
                //        { 
                //            options.BaseAddress = new Uri(configuration.GetSection("CatalogUrl").Value); 
                //        })
                //    .AddHttpMessageHandler<HttpClientAuthorizationDelegatingHandler>()
                //    .AddTypedClient(Refit.RestService.For<ICatalogServiceRefit>);
                .AddPolicyHandler(PollyExtensions.WaitAndRetry());


            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<IUser, AspNetUser>();
        }
    }

    public class PollyExtensions
    {
        public static AsyncRetryPolicy<HttpResponseMessage> WaitAndRetry()
        {
            var retry = HttpPolicyExtensions
                .HandleTransientHttpError()
                .WaitAndRetryAsync(new[] 
                {
                    TimeSpan.FromSeconds(1),
                    TimeSpan.FromSeconds(5),
                    TimeSpan.FromSeconds(10),
                },(outcome, timespan, retryCount, context) => 
                {
                    Console.WriteLine($"try per {retryCount } times");
                });

            return retry;
        }
    }
}
