using System.Collections.Generic;
using System.Threading;
using Microsoft.AspNetCore.SignalR.Client;
using REDTransport.NET.Message;
using REDTransport.NET.PushNotification;

namespace REDTransport.NET.SignalR
{
    public class SignalRClient<T> : IPushNotificationClient
    {
        public HubConnection HubConnection { get; protected set; }


        public SignalRClient(HubConnection hubConnection)
        {
            HubConnection = hubConnection;
        }

        public async IAsyncEnumerable<ResponseMessage> Listen(CancellationToken cancellationToken)
        {
            yield break;
        }
    }
}