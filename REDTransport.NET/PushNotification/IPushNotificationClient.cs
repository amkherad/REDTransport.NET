using System;
using REDTransport.NET.Tasks;

namespace REDTransport.NET.PushNotification
{
    public interface IPushNotificationClient
    {
        void RegisterCallback(Action<Message> callback);
    }
}