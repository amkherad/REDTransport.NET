using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using REDTransport.NET.Collections;
using REDTransport.NET.Exceptions;
using REDTransport.NET.Http;
using REDTransport.NET.Messages;
using REDTransport.NET.Server.AspNet.Message;

namespace REDTransport.NET.Server.AspNet.Pipeline
{
    public class RedTransportHttpChannelRequestDispatcher : IRedTransportRequestDispatcher
    {
        public RedTransportMiddlewareConfiguration Configuration { get; }

        public HttpClient HttpClient { get; }

        public IRedTransportMessageConverter<HttpRequestMessage, HttpResponseMessage> MessageConverter { get; }

        public MultipartMessageReaderWriter MultipartMessageReaderWriter { get; }

        public SystemTextJsonMessageReaderWriter JsonMessageReaderWriter { get; }


        public RedTransportHttpChannelRequestDispatcher(
            RedTransportMiddlewareConfiguration config,
            IRedTransportMessageConverter<HttpRequestMessage, HttpResponseMessage> messageConverter,
            MultipartMessageReaderWriter multipartMessageReaderWriter,
            SystemTextJsonMessageReaderWriter jsonMessageReaderWriter
        )
        {
            Configuration = config ?? throw new ArgumentNullException(nameof(config));
            MessageConverter = messageConverter ?? throw new ArgumentNullException(nameof(messageConverter));
            MultipartMessageReaderWriter = multipartMessageReaderWriter ??
                                           throw new ArgumentNullException(nameof(multipartMessageReaderWriter));
            JsonMessageReaderWriter = jsonMessageReaderWriter ??
                                      throw new ArgumentNullException(nameof(jsonMessageReaderWriter));
            HttpClient = new HttpClient();
        }

        public RedTransportHttpChannelRequestDispatcher(
            RedTransportMiddlewareConfiguration config,
            HttpClient httpClient,
            IRedTransportMessageConverter<HttpRequestMessage, HttpResponseMessage> messageConverter,
            MultipartMessageReaderWriter multipartMessageReaderWriter, SystemTextJsonMessageReaderWriter jsonMessageReaderWriter)
        {
            Configuration = config ?? throw new ArgumentNullException(nameof(config));
            HttpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            MessageConverter = messageConverter ?? throw new ArgumentNullException(nameof(messageConverter));
            MultipartMessageReaderWriter = multipartMessageReaderWriter ??
                                           throw new ArgumentNullException(nameof(multipartMessageReaderWriter));
            JsonMessageReaderWriter = jsonMessageReaderWriter ??
                                      throw new ArgumentNullException(nameof(jsonMessageReaderWriter));
        }

        private async ValueTask<ResponseMessage> MakeHttpCall(
            RequestMessage msg,
            CancellationToken cancellationToken
        )
        {
            var request = await MessageConverter.ToRequestAsync(msg, cancellationToken);

            var response = await HttpClient.SendAsync(request, cancellationToken);

            return await MessageConverter.FromResponseAsync(response, cancellationToken);
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
            if (contentType != null && contentType.StartsWith("multipart/"))
            {
                readerWriter = MultipartMessageReaderWriter;
            }
            else //if (contentType == "application/json" || contentType == "text/json")
            {
                readerWriter = JsonMessageReaderWriter;
            }
//            else
//            {
//                throw new RedTransportUnknownContentTypeException();
//            }

            var responseStream = context.Response.Body;

            if (message is RequestAggregationMessage aggregationMessage)
            {
                var requestToResponseAdapter = new AsyncEnumerableAdapter<RequestMessage, ResponseMessage>(
                    aggregationMessage.UnpackAsync(cancellationToken),
                    request => MakeHttpCall(request, cancellationToken)
                );

                await readerWriter.WriteResponseMessageToStream(responseStream, requestToResponseAdapter,
                    cancellationToken);
            }
            else
            {
                var response = await MakeHttpCall(message, cancellationToken);

                await readerWriter.WriteResponseMessageToStream(responseStream, response, cancellationToken);
            }
        }
    }
}