using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using REDTransport.NET.Exceptions;
using REDTransport.NET.Http;
using REDTransport.NET.Server.AspNet.Pipeline;

namespace REDTransport.NET.Server.AspNet
{
    public class RedTransportMiddleware : IMiddleware
    {
        public RedTransportMiddlewareConfiguration Configuration { get; }


        public RedTransportMiddleware(RedTransportMiddlewareConfiguration configuration)
        {
            Configuration = configuration;
        }


        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var messageConverter = context.RequestServices
                .GetService<IRedTransportMessageConverter<HttpRequest, HttpResponse>>();
            
            if (messageConverter == null)
            {
                throw new RedTransportException("MessageConverterIsNull");
            }

            if (
                messageConverter.IsRedRequest(context.Request) ||
                Configuration.Endpoints.Any(e => e.Value.IsMatched(context.Request.Path))
            )
            {
                var cancellationTokenSource = new CancellationTokenSource();

                var message = await messageConverter.FromRequestAsync(context.Request, cancellationTokenSource.Token);

                var dispatcher = context.RequestServices.GetService<IRedTransportRequestDispatcher>();

                if (dispatcher == null)
                {
                    throw new RedTransportException("DispatcherIsNull",
                        "Request for an instance of " +
                        nameof(IRedTransportRequestDispatcher) +
                        " has been failed."
                    );
                }

                await dispatcher.DispatchRedRequest(context, next, message, cancellationTokenSource.Token);

                return;
            }

            await next(context);
        }
    }
}