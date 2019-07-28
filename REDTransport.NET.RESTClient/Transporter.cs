using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using REDTransport.NET.Message;
using REDTransport.NET.PushNotification;
using REDTransport.NET.Tasks;

namespace REDTransport.NET.RESTClient
{
    public class Transporter
    {
        public HttpClient HttpClient { get; protected set; }

        public TaskTracker<HttpResponseMessage> TaskTracker { get; protected set; }

        public IPushNotificationClient PushNotificationClient { get; set; }


        public Transporter(
            HttpClient httpClient,
            TaskTracker<HttpResponseMessage> taskTracker,
            IPushNotificationClient pushNotificationClient
        )
        {
            if (taskTracker == null) throw new ArgumentNullException(nameof(taskTracker));
            if (httpClient == null) throw new ArgumentNullException(nameof(httpClient));
            if (pushNotificationClient == null) throw new ArgumentNullException(nameof(pushNotificationClient));

            HttpClient = httpClient;
            TaskTracker = taskTracker;
            PushNotificationClient = pushNotificationClient;
        }

        public Transporter(
            ITaskTrackerPersistentStorage<HttpResponseMessage> persistentStorage,
            IPushNotificationClient pushNotificationClient
        )
        {
            if (persistentStorage == null) throw new ArgumentNullException(nameof(persistentStorage));
            if (pushNotificationClient == null) throw new ArgumentNullException(nameof(pushNotificationClient));

            HttpClient = new HttpClient();
            TaskTracker = new TaskTracker<HttpResponseMessage>(persistentStorage);
        }


        public async Task Start(CancellationToken cancellationToken)
        {
            await foreach (var message in PushNotificationClient.Listen(cancellationToken))
            {
                if (TaskTracker.TryGetTaskByUniqueId(message.CorrelationId, out var taskInfo))
                {
                    taskInfo.TaskCompletionSource.SetResult(message.HttpResponseMessage);
                }
                else
                {
                    //TODO: Unknown/Custom message received in push notification client.
                    //It's either legal and should be handled by user, or illegal and should be dropped.
                }
            }
        }


        // OK-OK
        // Yield
        public Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage message,
            CancellationToken cancellationToken)
        {
            var taskCompletionSource = new TaskCompletionSource<HttpResponseMessage>();

            cancellationToken.Register(() => taskCompletionSource.SetCanceled());

            var task = HttpClient.SendAsync(message, cancellationToken)
                .ContinueWith(response =>
                {
                    if (response.IsFaulted)
                    {
                        taskCompletionSource.SetException(response.Exception);
                        return;
                    }

                    if (response.IsCanceled)
                    {
                        taskCompletionSource.SetCanceled();
                        return;
                    }

                    if (!response.Result.Headers.TryGetValues(Protocol.REDResponseActionHeaderName,
                        out var responseActions))
                    {
                        responseActions = new[] {ResponseActions.Normal};
                    }

                    switch (responseActions.Single())
                    {
                        case ResponseActions.Yield:
                        {
                            var correlationId = response.Result.Headers.GetValues(Protocol.REDCorrelationIdHeaderName)
                                .SingleOrDefault();

                            if (string.IsNullOrWhiteSpace(correlationId))
                            {
                                throw new ProtocolException(ProtocolException.YieldCorrelationIdIsMissing);
                            }

                            TimeSpan? timeout = null;
                            if (response.Result.Headers.TryGetValues(Protocol.REDYieldTimeoutHeaderName,
                                out var yieldTimeoutStrings))
                            {
                                var timeoutStr = yieldTimeoutStrings.SingleOrDefault();

                                timeout = TimeSpan.Parse(timeoutStr);
                            }

                            var trackedTask = new TaskInfo<HttpResponseMessage>(taskCompletionSource, timeout);

                            if (!cancellationToken.IsCancellationRequested)
                            {
                                TaskTracker.Track(correlationId, trackedTask);
                            }

                            break;
                        }

                        //case ResponseActions.Normal:
                        default:
                        {
                            taskCompletionSource.SetResult(default);

                            break;
                        }
                    }
                }, cancellationToken).ConfigureAwait(false);

            return taskCompletionSource.Task;
        }
    }
}