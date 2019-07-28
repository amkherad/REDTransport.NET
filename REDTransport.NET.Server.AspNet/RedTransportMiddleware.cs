using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace REDTransport.NET.Server.AspNet
{
    public class RedTransportMiddleware : IMiddleware
    {
        public Task InvokeAsync(HttpContext context, RequestDelegate next)
        {

            return next(context);
        }
    }
}