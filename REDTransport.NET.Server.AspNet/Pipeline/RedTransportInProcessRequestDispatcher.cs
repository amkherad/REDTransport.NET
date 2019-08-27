using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Http.Features.Authentication;
using Microsoft.Extensions.DependencyInjection;
using REDTransport.NET.Collections;
using REDTransport.NET.Http;
using REDTransport.NET.Messages;
using REDTransport.NET.Server.AspNet.Message;

namespace REDTransport.NET.Server.AspNet.Pipeline
{
    public partial class RedTransportInProcessRequestDispatcher : IRedTransportRequestDispatcher
    {
        //public IHttpApplication<> Application { get; }

        public RedTransportMiddlewareConfiguration Configuration { get; }


        public IRedTransportMessageConverter<HttpRequest, HttpResponse> MessageConverter { get; }

        public MultipartMessageReaderWriter MultipartMessageReaderWriter { get; }

        public SystemTextJsonMessageReaderWriter JsonMessageReaderWriter { get; }


        public RedTransportInProcessRequestDispatcher(
            //IHttpApplication<HostingApplication.Context> application,
            RedTransportMiddlewareConfiguration configuration,
            IRedTransportMessageConverter<HttpRequest, HttpResponse> messageConverter,
            MultipartMessageReaderWriter multipartMessageReaderWriter,
            SystemTextJsonMessageReaderWriter jsonMessageReaderWriter
        )
        {
            //Application = application;
            Configuration = configuration;
            MessageConverter = messageConverter;
            MultipartMessageReaderWriter =
                multipartMessageReaderWriter ?? throw new ArgumentNullException(nameof(multipartMessageReaderWriter));
            JsonMessageReaderWriter =
                jsonMessageReaderWriter ?? throw new ArgumentNullException(nameof(jsonMessageReaderWriter));
        }

        public async Task DispatchRedRequest(
            HttpContext context,
            RequestDelegate next,
            RequestMessage message,
            CancellationToken cancellationToken
        )
        {
            var contentType = message.Headers.ContentType;

            var responseMessage = new ResponseMessage();

            IMessageReaderWriter readerWriter;
            if (contentType != null && contentType.StartsWith("multipart/"))
            {
                readerWriter = MultipartMessageReaderWriter;
                responseMessage.Headers.ContentType = "multipart/mixed";
            }
            else //if (contentType == "application/json" || contentType == "text/json")
            {
                readerWriter = JsonMessageReaderWriter;
                responseMessage.Headers.ContentType = contentType == "text/json" ? contentType : "application/json";
            }

            var response = context.Response;
            var responseStream = context.Response.Body;

            if (message is RequestAggregationMessage aggregationMessage)
            {
                var requestToResponseAdapter = new AsyncEnumerableAdapter<RequestMessage, ResponseMessage>(
                    aggregationMessage.UnpackAsync(cancellationToken),
                    request => ProgressPipeline(context, next, request, responseMessage, cancellationToken)
                );

                var responseContentType = readerWriter.GetResponseContentTypeFromRequestContentType(contentType);

                var headers = response.Headers;
                headers.Add("Content-Type", responseContentType);

                await readerWriter.WriteResponseMessageToStream(responseStream, requestToResponseAdapter,
                    cancellationToken);
            }
            else
            {
                var pipelineResponse =
                    await ProgressPipeline(context, next, message, responseMessage, cancellationToken);

                await readerWriter.WriteResponseMessageToStream(responseStream, pipelineResponse, cancellationToken);

                await MessageConverter.CopyResponseMessageToTarget(response, pipelineResponse, cancellationToken);
            }
        }


        private async ValueTask<ResponseMessage> ProgressPipeline(
            HttpContext context,
            RequestDelegate next,
            RequestMessage requestMessage,
            ResponseMessage responseMessage,
            CancellationToken cancellationToken
        )
        {
            var requestFeature = new RequestFeature(requestMessage);
            var responseFeature = new ResponseFeature(responseMessage);

            var requestLifetimeFeature = new HttpRequestLifetimeFeature
            {
                RequestAborted = cancellationToken
            };

            var serviceProviderFeature = context.Features.Get<IServiceProvidersFeature>();
            var requestAuthenticationFeature = context.Features.Get<IHttpAuthenticationFeature>();
            var formFeature = context.Features.Get<IFormFeature>();
            var responseTrailersFeature = context.Features.Get<IHttpResponseTrailersFeature>();
            //var httpResponseBodyFeature = context.Features.Get<IHttpResponseBodyFeature>();
            //var httpBodyControlFeature = context.Features.Get<IHttpResponseBodyPipeFeature>();
            //var httpResponseBodyFeature = context.Features.Get<IHttpResponseFeature>();

            GC.KeepAlive(context.Features);

            var features = new FeatureCollection();

            features.Set<IHttpRequestFeature>(new HttpRequestFeature());
            features.Set<IHttpResponseFeature>(new HttpResponseFeature());
            
//            features.Set<IHttpRequestFeature>(requestFeature);
//            features.Set<IHttpResponseFeature>(responseFeature);
            
            features.Set<IHttpRequestLifetimeFeature>(requestLifetimeFeature);
            features.Set<IHttpAuthenticationFeature>(requestAuthenticationFeature);
            features.Set<IServiceProvidersFeature>(serviceProviderFeature);
            features.Set<IFormFeature>(formFeature);
            features.Set<IHttpResponseTrailersFeature>(responseTrailersFeature);
            //features.Set<IHttpBodyControlFeature>(httpBodyControlFeature);
            //features.Set<IHttpResponseBodyFeature>(httpResponseBodyFeature);
            //features.Set<IHttpResponseBodyPipeFeature>(null);

            var ctx = new DefaultHttpContext(features);

            await MessageConverter.CopyRequestMessageToTarget(ctx.Request, requestMessage, cancellationToken);

            IServiceProvider serviceProvider;

            if (Configuration.InProcessScopeMode != RedTransportInProcessScopeMode.UseRootScope)
            {
                //creating a new scope for IServiceProvider.

                var factory = serviceProviderFeature.RequestServices.GetRequiredService<IServiceScopeFactory>();

                serviceProvider = factory.CreateScope().ServiceProvider;

                serviceProviderFeature = new ServiceProvidersFeature
                {
                    RequestServices = serviceProvider
                };
                features.Set<IServiceProvidersFeature>(serviceProviderFeature);
            }
            else
            {
                serviceProvider = serviceProviderFeature.RequestServices;
            }

            var memStream = new MemoryStream();

            ctx.Response.Body = memStream;
            
            //GC.KeepAlive(ctx.Response.Body);

            ctx.Response.RegisterForDispose(memStream);

            await next(ctx);

            return await MessageConverter.FromResponseAsync(ctx.Response, cancellationToken);
        }
    }
}