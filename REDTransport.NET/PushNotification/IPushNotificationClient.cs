using System.Collections.Generic;
using System.Threading;
using REDTransport.NET.Message;

namespace REDTransport.NET.PushNotification
{
    public interface IPushNotificationClient
    {
        IAsyncEnumerable<ResponseMessage> Listen(CancellationToken cancellationToken);
    }
}