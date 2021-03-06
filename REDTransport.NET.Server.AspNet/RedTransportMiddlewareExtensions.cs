using System;
using System.Net.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using REDTransport.NET.Http;
using REDTransport.NET.Server.AspNet.Message;
using REDTransport.NET.Server.AspNet.Pipeline;

namespace REDTransport.NET.Server.AspNet
{
    public static class RedTransportMiddlewareExtensions
    {
        public static void AddRedTransport(
            this IServiceCollection services,
            Action<RedTransportMiddlewareConfiguration> configBuilder
        )
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            if (configBuilder == null) throw new ArgumentNullException(nameof(configBuilder));

            var config = new RedTransportMiddlewareConfiguration();

            configBuilder(config);

            switch (config.RequestDispatcherStrategy)
            {
                case RequestDispatcherStrategy.InProcess:
                    services.AddSingleton<IRedTransportRequestDispatcher, RedTransportInProcessRequestDispatcher>();
                    break;
                case RequestDispatcherStrategy.HttpChannel:
                    services
                        .AddSingleton<IRedTransportRequestDispatcher, RedTransportHttpChannelRequestDispatcher>();
                    break;
                case RequestDispatcherStrategy.Custom:
                    break;
                default:
                    throw new IndexOutOfRangeException(nameof(config.RequestDispatcherStrategy));
            }

            services.AddSingleton(config);

            services.AddSingleton<RedTransportMiddleware>();

            services
                .AddSingleton<IRedTransportMessageConverter<HttpRequest, HttpResponse>,
                    DefaultMessage2AspNetCoreHttpConverter>();
            services
                .AddSingleton<IRedTransportMessageConverter<HttpRequestMessage, HttpResponseMessage>,
                    DefaultMessage2NetHttpConverter>();

            services.AddSingleton<MultipartMessageReaderWriter>();
            services.AddSingleton<SystemTextJsonMessageReaderWriter>();

            //services.AddTransient<IRedTransportRequestDispatcher, RedTransportRequestDispatcher>();
        }

        public static void UseRedTransport(
            this IApplicationBuilder app
        )
        {
            app.UseMiddleware<RedTransportMiddleware>();
        }
    }
}