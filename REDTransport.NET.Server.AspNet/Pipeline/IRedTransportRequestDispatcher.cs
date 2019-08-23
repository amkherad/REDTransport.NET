using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using REDTransport.NET.Messages;

namespace REDTransport.NET.Server.AspNet.Pipeline
{
    public interface IRedTransportRequestDispatcher
    {
        Task DispatchRedRequest(
            HttpContext context,
            RequestDelegate next,
            RequestMessage message,
            CancellationToken cancellationToken
        );
    }
}