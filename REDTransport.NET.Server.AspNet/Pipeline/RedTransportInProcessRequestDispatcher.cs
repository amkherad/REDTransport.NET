using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Http;
using REDTransport.NET.Collections;
using REDTransport.NET.Exceptions;
using REDTransport.NET.Http;
using REDTransport.NET.Messages;
using REDTransport.NET.Server.AspNet.Message;

namespace REDTransport.NET.Server.AspNet.Pipeline
{
    public class RedTransportInProcessRequestDispatcher : IRedTransportRequestDispatcher
    {
        public IHttpApplication<HostingApplication.Context> Application { get; }

        public RedTransportMiddlewareConfiguration Configuration { get; }


        public IRedTransportMessageConverter<HttpRequest, HttpResponse> MessageConverter { get; }

        public MultipartMessageReaderWriter MultipartMessageReaderWriter { get; }

        public JsonMessageReaderWriter JsonMessageReaderWriter { get; }


        public RedTransportInProcessRequestDispatcher(
            IHttpApplication<HostingApplication.Context> application,
            RedTransportMiddlewareConfiguration configuration,
            IRedTransportMessageConverter<HttpRequest, HttpResponse> messageConverter,
            MultipartMessageReaderWriter multipartMessageReaderWriter,
            JsonMessageReaderWriter jsonMessageReaderWriter
        )
        {
            Application = application;
            Configuration = configuration;
            MessageConverter = messageConverter;
            MultipartMessageReaderWriter = multipartMessageReaderWriter ??
                                           throw new ArgumentNullException(nameof(multipartMessageReaderWriter));
            JsonMessageReaderWriter = jsonMessageReaderWriter ??
                                      throw new ArgumentNullException(nameof(jsonMessageReaderWriter));
        }

        public async Task DispatchRedRequest(
            HttpContext context,
            RequestDelegate next,
            RequestMessage message,
            CancellationToken cancellationToken
        )
        {
            var contentType = message.Headers.ContentType;

            IMessageReaderWriter readerWriter;
            if (contentType.StartsWith("multipart/"))
            {
                readerWriter = MultipartMessageReaderWriter;
            }
            else if (contentType == "application/json" || contentType == "text/json")
            {
                readerWriter = JsonMessageReaderWriter;
            }
            else
            {
                throw new RedTransportUnknownContentTypeException();
            }

            var pipeline = Configuration.InProcessScopeMode == RedTransportInProcessScopeMode.UseRootScope
                ? (Func<HttpContext, RequestDelegate, RequestMessage, CancellationToken, ValueTask<ResponseMessage>>)
                ProgressPipeline
                : ProgressPipelinePerScope;


            var responseStream = context.Response.Body;

            if (message is RequestAggregationMessage aggregationMessage)
            {
                var requestToResponseAdapter = new AsyncEnumerableAdapter<RequestMessage, ResponseMessage>(
                    aggregationMessage.UnpackAsync(cancellationToken),
                    request => pipeline(context, next, request, cancellationToken)
                );

                await readerWriter.WriteResponseMessageToStream(responseStream, requestToResponseAdapter,
                    cancellationToken);
            }
            else
            {
                var response = await pipeline(context, next, message, cancellationToken);

                await readerWriter.WriteResponseMessageToStream(responseStream, response, cancellationToken);
            }
        }


        private async ValueTask<ResponseMessage> ProgressPipelinePerScope(
            HttpContext context,
            RequestDelegate next,
            RequestMessage msg,
            CancellationToken cancellationToken
        )
        {
            var nCtx = Application.CreateContext(context.Features);

            var request = nCtx.HttpContext.Request;

            request.Protocol = msg.ProtocolVersion;
            request.Method = msg.RequestMethod;


            await Application.ProcessRequestAsync(nCtx);


            return await MessageConverter.FromResponseAsync(nCtx.HttpContext.Response, cancellationToken);
        }


        private async ValueTask<ResponseMessage> ProgressPipeline(
            HttpContext context,
            RequestDelegate next,
            RequestMessage msg,
            CancellationToken cancellationToken
        )
        {
            var nCtx = Application.CreateContext(context.Features);

            var request = nCtx.HttpContext.Request;

            request.Protocol = msg.ProtocolVersion;
            request.Method = msg.RequestMethod;


            await Application.ProcessRequestAsync(nCtx);


            return await MessageConverter.FromResponseAsync(nCtx.HttpContext.Response, cancellationToken);
        }


//        internal class HttpContextBuilder
//        {
//            private readonly IHttpApplication<HostingApplication.Context> _application;
//            private readonly HttpContext _httpContext;
//
//            private TaskCompletionSource<HttpContext> _responseTcs =
//                new TaskCompletionSource<HttpContext>(TaskCreationOptions.RunContinuationsAsynchronously);
//
//            private ResponseStream _responseStream;
//            private ResponseFeature _responseFeature = new ResponseFeature();
//            private CancellationTokenSource _requestAbortedSource = new CancellationTokenSource();
//            private bool _pipelineFinished;
//            private HostingApplication.Context _testContext;
//
//            internal HttpContextBuilder(IHttpApplication<HostingApplication.Context> application)
//            {
//                _application = application ?? throw new ArgumentNullException(nameof(application));
//                _httpContext = new DefaultHttpContext();
//
//                var request = _httpContext.Request;
//                request.Protocol = "HTTP/1.1";
//                request.Method = HttpMethods.Get;
//
//                _httpContext.Features.Set<IHttpResponseFeature>(_responseFeature);
//                var requestLifetimeFeature = new HttpRequestLifetimeFeature();
//                requestLifetimeFeature.RequestAborted = _requestAbortedSource.Token;
//                _httpContext.Features.Set<IHttpRequestLifetimeFeature>(requestLifetimeFeature);
//
//                
//                IHttpResponseFeature xx = new HttpResponseFeature();
//                
//                _responseStream = new ResponseStream(ReturnResponseMessageAsync, AbortRequest);
//                _responseFeature.Body = _responseStream;
//            }
//
//            internal void Configure(Action<HttpContext> configureContext)
//            {
//                if (configureContext == null)
//                {
//                    throw new ArgumentNullException(nameof(configureContext));
//                }
//
//                configureContext(_httpContext);
//            }
//
//            /// <summary>
//            /// Start processing the request.
//            /// </summary>
//            /// <returns></returns>
//            internal Task<HttpContext> SendAsync(CancellationToken cancellationToken)
//            {
//                var registration = cancellationToken.Register(AbortRequest);
//
//                _testContext = _application.CreateContext(_httpContext.Features);
//
//                // Async offload, don't let the test code block the caller.
//                _ = Task.Factory.StartNew(async () =>
//                {
//                    try
//                    {
//                        await _application.ProcessRequestAsync(_testContext);
//                        await CompleteResponseAsync();
//                        _application.DisposeContext(_testContext, exception: null);
//                    }
//                    catch (Exception ex)
//                    {
//                        Abort(ex);
//                        _application.DisposeContext(_testContext, ex);
//                    }
//                    finally
//                    {
//                        registration.Dispose();
//                    }
//                });
//
//                return _responseTcs.Task;
//            }
//
//            internal void AbortRequest()
//            {
//                if (!_pipelineFinished)
//                {
//                    _requestAbortedSource.Cancel();
//                }
//
//                _responseStream.CompleteWrites();
//            }
//
//            internal async Task CompleteResponseAsync()
//            {
//                _pipelineFinished = true;
//                await ReturnResponseMessageAsync();
//                _responseStream.CompleteWrites();
//                await _responseFeature.FireOnResponseCompletedAsync();
//            }
//
//            internal async Task ReturnResponseMessageAsync()
//            {
//                // Check if the response has already started because the TrySetResult below could happen a bit late
//                // (as it happens on a different thread) by which point the CompleteResponseAsync could run and calls this
//                // method again.
//                if (!_responseFeature.HasStarted)
//                {
//                    // Sets HasStarted
//                    try
//                    {
//                        await _responseFeature.FireOnSendingHeadersAsync();
//                    }
//                    catch (Exception ex)
//                    {
//                        Abort(ex);
//                        return;
//                    }
//
//                    // Copy the feature collection so we're not multi-threading on the same collection.
//                    var newFeatures = new FeatureCollection();
//                    foreach (var pair in _httpContext.Features)
//                    {
//                        newFeatures[pair.Key] = pair.Value;
//                    }
//
//                    _responseTcs.TrySetResult(new DefaultHttpContext(newFeatures));
//                }
//            }
//
//            internal void Abort(Exception exception)
//            {
//                _pipelineFinished = true;
//                _responseStream.Abort(exception);
//                _responseTcs.TrySetException(exception);
//            }
//        }
    }
}