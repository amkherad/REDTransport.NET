using System;
using System.Threading;
using System.Threading.Tasks;
using REDTransport.NET.Message;

namespace REDTransport.NET.Server.AspNet
{
    public class RedTransportRequestDispatcher
    {
        public IServiceProvider ServiceProvider { get; }

        public async Task DispatchRedRequest(RequestMessage message, CancellationToken cancellationToken)
        {
            if (message is RequestAggregationMessage aggregationMessage)
            {
                await foreach (var msg in aggregationMessage.UnpackAsync(
                    ServiceProvider,
                    cancellationToken
                ))
                {
                    
                }
            }
        }
    }
}