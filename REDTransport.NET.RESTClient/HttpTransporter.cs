using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using REDTransport.NET.Http;
using REDTransport.NET.Message;
using REDTransport.NET.PushNotification;
using REDTransport.NET.Tasks;

namespace REDTransport.NET.RESTClient
{
    public class HttpTransporter
    {
        public HttpClient HttpClient { get; protected set; }

        public TaskTracker<ResponseMessage> TaskTracker { get; protected set; }

        public IPushNotificationClient PushNotificationClient { get; set; }

        public HttpConverter HttpConverter { get; }


        public HttpTransporter(
            HttpClient httpClient,
            TaskTracker<ResponseMessage> taskTracker,
            IPushNotificationClient pushNotificationClient,
            HttpConverter httpConverter
        )
        {
            if (taskTracker == null) throw new ArgumentNullException(nameof(taskTracker));
            if (httpClient == null) throw new ArgumentNullException(nameof(httpClient));
            if (pushNotificationClient == null) throw new ArgumentNullException(nameof(pushNotificationClient));

            HttpClient = httpClient;
            TaskTracker = taskTracker;
            PushNotificationClient = pushNotificationClient;
            HttpConverter = httpConverter;
        }

        public HttpTransporter(
            ITaskTrackerPersistentStorage<ResponseMessage> persistentStorage,
            IPushNotificationClient pushNotificationClient, HttpConverter httpConverter
        )
        {
            if (persistentStorage == null) throw new ArgumentNullException(nameof(persistentStorage));
            if (pushNotificationClient == null) throw new ArgumentNullException(nameof(pushNotificationClient));
            HttpConverter = httpConverter;

            HttpClient = new HttpClient();
            TaskTracker = new TaskTracker<ResponseMessage>(persistentStorage);
        }


        public async Task Start(CancellationToken cancellationToken)
        {
            await foreach (var message in PushNotificationClient.Listen(cancellationToken))
            {
                if (TaskTracker.TryGetTaskByUniqueId(message.CorrelationId, out var taskInfo))
                {
                    taskInfo.TaskCompletionSource.SetResult(message);
                }
                else
                {
                    //TODO: Unknown/Custom message received in push notification client.
                    //It's either legal and should be handled by user, or illegal and should be dropped.
                }
            }
        }


        public Task<Tuple<ResponseMessage, Task<ResponseMessage>>> SendExAsync(
            RequestMessage message,
            CancellationToken cancellationToken)
        {
            var taskCompletionSource =
                new TaskCompletionSource<Tuple<ResponseMessage, Task<ResponseMessage>>>();

            cancellationToken.Register(() => taskCompletionSource.SetCanceled());

            var httpRequestMessage = HttpConverter.ToHttpRequestMessage(message);
            
            var task = HttpClient.SendAsync(httpRequestMessage, cancellationToken)
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

                            if (!cancellationToken.IsCancellationRequested)
                            {
                                var innerTask = new TaskCompletionSource<ResponseMessage>();

                                var trackedTask = new TaskInfo<ResponseMessage>(innerTask, timeout);

                                TaskTracker.Track(correlationId, trackedTask);

                                var responseMessage = HttpConverter.ToHttpResponseMessage(response.Result);
                                
                                taskCompletionSource.SetResult(
                                    new Tuple<ResponseMessage, Task<ResponseMessage>>(
                                        ,
                                        innerTask.Task
                                    )
                                );
                            }

                            break;
                        }

                        //case ResponseActions.Normal:
                        default:
                        {
                            taskCompletionSource.SetResult(
                                new Tuple<ResponseMessage, Task<ResponseMessage>>(response.Result, null)
                            );

                            break;
                        }
                    }
                }, cancellationToken).ConfigureAwait(false);

            return taskCompletionSource.Task;
        }

        public Task<ResponseMessage> SendAsync(
            RequestMessage message,
            CancellationToken cancellationToken)
        {
            var taskCompletionSource = new TaskCompletionSource<ResponseMessage>();

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

                            var trackedTask = new TaskInfo<ResponseMessage>(taskCompletionSource, timeout);

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