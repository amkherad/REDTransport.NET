using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using REDTransport.NET.Exceptions;
using REDTransport.NET.Http;
using REDTransport.NET.Messages;
using REDTransport.NET.PushNotification;
using REDTransport.NET.Tasks;

namespace REDTransport.NET.RESTClient
{
    public class HttpTransporter
    {
        public const string REDProtocolVersion = "1.00";

        public HttpClient HttpClient { get; protected set; }

        public InMemoryTaskTracker<ResponseMessage> InMemoryTaskTracker { get; protected set; }

        public IPushNotificationClient PushNotificationClient { get; set; }

        public IRedTransportMessageConverter<HttpRequestMessage, HttpResponseMessage> HttpConverter { get; }


        public string ProtocolVersion { get; } = "HTTP/1.1";


        public HttpTransporter(
            HttpClient httpClient,
            IRedTransportMessageConverter<HttpRequestMessage, HttpResponseMessage> httpConverter
        )
        {
            HttpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            HttpConverter = httpConverter ?? throw new ArgumentNullException(nameof(httpConverter));

            InMemoryTaskTracker = new InMemoryTaskTracker<ResponseMessage>();
        }

        public HttpTransporter(
            HttpClient httpClient,
            InMemoryTaskTracker<ResponseMessage> taskTracker,
            IPushNotificationClient pushNotificationClient,
            IRedTransportMessageConverter<HttpRequestMessage, HttpResponseMessage> httpConverter
        )
        {
            HttpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            InMemoryTaskTracker = taskTracker ?? throw new ArgumentNullException(nameof(taskTracker));
            PushNotificationClient =
                pushNotificationClient ?? throw new ArgumentNullException(nameof(pushNotificationClient));
            HttpConverter = httpConverter ?? throw new ArgumentNullException(nameof(httpConverter));
        }

        public HttpTransporter(
            ITaskTrackerPersistentStorage<ResponseMessage> persistentStorage,
            IPushNotificationClient pushNotificationClient,
            IRedTransportMessageConverter<HttpRequestMessage, HttpResponseMessage> httpConverter
        )
        {
            if (persistentStorage == null) throw new ArgumentNullException(nameof(persistentStorage));

            PushNotificationClient =
                pushNotificationClient ?? throw new ArgumentNullException(nameof(pushNotificationClient));
            HttpClient = new HttpClient();
            InMemoryTaskTracker = new InMemoryTaskTracker<ResponseMessage>(persistentStorage);
            HttpConverter = httpConverter;
        }


        public async Task Start(CancellationToken cancellationToken)
        {
            if (PushNotificationClient == null)
            {
                throw new RedTransportException("PushNotificationClientIsNull");
            }

            await foreach (var message in PushNotificationClient.Listen(cancellationToken))
            {
                if (InMemoryTaskTracker.TryGetTaskByUniqueId(message.CorrelationId, out var taskInfo))
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


        public async Task<Tuple<ResponseMessage, Task<ResponseMessage>>> SendExAsync(
            RequestMessage message,
            CancellationToken cancellationToken)
        {
            var taskCompletionSource =
                new TaskCompletionSource<Tuple<ResponseMessage, Task<ResponseMessage>>>();

            cancellationToken.Register(() => taskCompletionSource.SetCanceled());

            var httpRequestMessage = await HttpConverter.ToRequestAsync(message, cancellationToken);

            httpRequestMessage.Headers.Add(ProtocolConstants.REDProtocolVersionHeaderName, REDProtocolVersion);

            var task = HttpClient.SendAsync(httpRequestMessage, cancellationToken)
                .ContinueWith(async response =>
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

                    if (!response.Result.Headers.TryGetValues(ProtocolConstants.REDResponseActionHeaderName,
                        out var responseActions))
                    {
                        responseActions = new[] {ResponseActions.Normal};
                    }

                    switch (responseActions.Single())
                    {
                        case ResponseActions.Yield:
                        {
                            var correlationId = response.Result.Headers
                                .GetValues(ProtocolConstants.REDCorrelationIdHeaderName)
                                .SingleOrDefault();

                            if (string.IsNullOrWhiteSpace(correlationId))
                            {
                                throw new ProtocolException(ProtocolException.YieldCorrelationIdIsMissing);
                            }

                            TimeSpan? timeout = null;
                            if (response.Result.Headers.TryGetValues(ProtocolConstants.REDYieldTimeoutHeaderName,
                                out var yieldTimeoutStrings))
                            {
                                var timeoutStr = yieldTimeoutStrings.SingleOrDefault();

                                timeout = TimeSpan.Parse(timeoutStr);
                            }

                            if (!cancellationToken.IsCancellationRequested)
                            {
                                var innerTask = new TaskCompletionSource<ResponseMessage>();

                                var trackedTask = new TaskInfo<ResponseMessage>(innerTask, timeout);

                                InMemoryTaskTracker.Track(correlationId, trackedTask);

                                var responseMessage =
                                    await HttpConverter.FromResponseAsync(response.Result, cancellationToken);

                                taskCompletionSource.SetResult(
                                    new Tuple<ResponseMessage, Task<ResponseMessage>>(
                                        responseMessage,
                                        innerTask.Task
                                    )
                                );
                            }

                            break;
                        }

                        //case ResponseActions.Normal:
                        default:
                        {
                            var responseMessage =
                                await HttpConverter.FromResponseAsync(response.Result, cancellationToken);

                            taskCompletionSource.SetResult(
                                new Tuple<ResponseMessage, Task<ResponseMessage>>(responseMessage, null)
                            );

                            break;
                        }
                    }
                }, cancellationToken).ConfigureAwait(false);

            return await taskCompletionSource.Task;
        }

        public async Task<ResponseMessage> SendAsync(
            RequestMessage message,
            CancellationToken cancellationToken)
        {
            var taskCompletionSource = new TaskCompletionSource<ResponseMessage>();

            cancellationToken.Register(() => taskCompletionSource.SetCanceled());

            var httpRequestMessage = await HttpConverter.ToRequestAsync(message, cancellationToken);

            httpRequestMessage.Headers.Add(ProtocolConstants.REDProtocolVersionHeaderName, REDProtocolVersion);

            var task = HttpClient.SendAsync(httpRequestMessage, cancellationToken)
                .ContinueWith(async response =>
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

                    if (!response.Result.Headers.TryGetValues(ProtocolConstants.REDResponseActionHeaderName,
                        out var responseActions))
                    {
                        responseActions = new[] {ResponseActions.Normal};
                    }

                    switch (responseActions.Single())
                    {
                        case ResponseActions.Yield:
                        {
                            var correlationId = response.Result.Headers
                                .GetValues(ProtocolConstants.REDCorrelationIdHeaderName)
                                .SingleOrDefault();

                            if (string.IsNullOrWhiteSpace(correlationId))
                            {
                                throw new ProtocolException(ProtocolException.YieldCorrelationIdIsMissing);
                            }

                            TimeSpan? timeout = null;
                            if (response.Result.Headers.TryGetValues(ProtocolConstants.REDYieldTimeoutHeaderName,
                                out var yieldTimeoutStrings))
                            {
                                var timeoutStr = yieldTimeoutStrings.SingleOrDefault();

                                timeout = TimeSpan.Parse(timeoutStr);
                            }

                            var trackedTask = new TaskInfo<ResponseMessage>(taskCompletionSource, timeout);

                            if (!cancellationToken.IsCancellationRequested)
                            {
                                InMemoryTaskTracker.Track(correlationId, trackedTask);
                            }

                            break;
                        }

                        //case ResponseActions.Normal:
                        default:
                        {
                            taskCompletionSource.SetResult(
                                await HttpConverter.FromResponseAsync(response.Result, cancellationToken)
                            );

                            break;
                        }
                    }
                }, cancellationToken).ConfigureAwait(false);

            return await taskCompletionSource.Task;
        }
    }
}