using System.Collections.Generic;
using System.Threading;
using REDTransport.NET.Messages;

namespace REDTransport.NET.PushNotification
{
    public interface IPushNotificationClient
    {
        IAsyncEnumerable<ResponseMessage> Listen(CancellationToken cancellationToken);
    }
}