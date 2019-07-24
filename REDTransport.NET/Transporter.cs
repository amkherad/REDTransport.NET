using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using REDTransport.NET.Http;
using REDTransport.NET.PushNotification;
using REDTransport.NET.Tasks;

namespace REDTransport.NET
{
    public class Transporter
    {
        public HttpClientWrapper HttpClient { get; protected set; }

        public TaskTracker TaskTracker { get; protected set; }

        public IPushNotificationClient PushNotificationClient { get; set; }


        public Transporter(TaskTracker taskTracker, IPushNotificationClient pushNotificationClient)
        {
            if (taskTracker == null) throw new ArgumentNullException(nameof(taskTracker));

            HttpClient = new HttpClientWrapper();
            TaskTracker = taskTracker;
            PushNotificationClient = pushNotificationClient;
        }

        public Transporter(TaskTracker taskTracker)
        {
            if (taskTracker == null) throw new ArgumentNullException(nameof(taskTracker));

            HttpClient = new HttpClientWrapper();
            TaskTracker = taskTracker;
        }

        public Transporter(ITaskTrackerPersistentStorage persistentStorage)
        {
            if (persistentStorage == null) throw new ArgumentNullException(nameof(persistentStorage));

            HttpClient = new HttpClientWrapper();
            TaskTracker = new TaskTracker(persistentStorage);
        }


        public void Start()
        {
            PushNotificationClient.RegisterCallback(message =>
            {
                if (TaskTracker.TryGetTaskByMessage(message, out var taskInfo))
                {
                    taskInfo.TaskCompletionSource.SetResult(message);
                }
            });
        }


        // OK-OK
        // Yield

        public Task<Message> SendAsync(EndPointDescriptor endPoint, Message message,
            CancellationToken cancellationToken)
        {
            var taskCompletionSource = new TaskCompletionSource<Message>();

            cancellationToken.Register(() => taskCompletionSource.SetCanceled());

            var task = HttpClient.GetAsync(endPoint, cancellationToken)
                .ContinueWith(response =>
                {
                    
                    taskCompletionSource.SetResult(default);
                    
                }, cancellationToken).ConfigureAwait(false);

            return taskCompletionSource.Task;
        }

        public async Task<T> SendAsync<T>(EndPointDescriptor endPoint, Message message,
            CancellationToken cancellationToken)
        {
            //var taskCompletionSource = new TaskCompletionSource<T>();

            //cancellationToken.Register(() => taskCompletionSource.SetCanceled());

//            HttpClient.GetAsync(endPoint, cancellationToken)
//                .ContinueWith(response =>
//                {
//                    
//                    taskCompletionSource.SetResult(default);
//                    
//                }, cancellationToken);

            //return taskCompletionSource.Task;
        }
    }
}