using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace REDTransport.NET.Server.AspNet
{
    public static class RedTransportMiddlewareExtensions
    {
        public static void AddRedTransport(
            this IServiceCollection services,
            Action<RedTransportMiddlewareConfiguration> configBuilder
        )
        {
            services.AddSingleton(service =>
            {
                var config = new RedTransportMiddlewareConfiguration();
                configBuilder(config);
                return config;
            });

            services.AddSingleton<RedTransportMiddleware>();
        }

        public static void UseRedTransport(
            this IApplicationBuilder app
        )
        {
            app.UseMiddleware<RedTransportMiddleware>();
        }
    }
}