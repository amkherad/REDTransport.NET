using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using REDTransport.NET.Messages;

namespace REDTransport.NET.Http
{
    public class
        DefaultMessage2NetHttpConverter : IRedTransportMessageConverter<HttpRequestMessage, HttpResponseMessage>
    {
        public bool IsRedRequest(HttpRequestMessage request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            if (request.Headers.TryGetValues(ProtocolConstants.REDProtocolVersionHeaderName, out var version))
            {
                return true;
            }

            return false;
        }

        public bool IsRedResponse(HttpResponseMessage response)
        {
            if (response == null) throw new ArgumentNullException(nameof(response));

            if (response.Headers.TryGetValues(ProtocolConstants.REDProtocolVersionHeaderName, out var version))
            {
                return true;
            }

            return false;
        }

        public Task<HttpRequestMessage> ToRequestAsync(RequestMessage requestMessage,
            CancellationToken cancellationToken)
        {
            if (requestMessage == null) throw new ArgumentNullException(nameof(requestMessage));

            var request = new HttpRequestMessage();

            CopyRequestMessageToTarget(request, requestMessage, cancellationToken);
            
            return Task.FromResult(request);
        }

        public Task<HttpResponseMessage> ToResponseAsync(ResponseMessage responseMessage,
            CancellationToken cancellationToken)
        {
            if (responseMessage == null) throw new ArgumentNullException(nameof(responseMessage));

            var response = new HttpResponseMessage();

            CopyResponseMessageToTarget(response, responseMessage, cancellationToken);

            return Task.FromResult(response);
        }

        public async Task<RequestMessage> FromRequestAsync(HttpRequestMessage requestMessage,
            CancellationToken cancellationToken)
        {
            if (requestMessage == null) throw new ArgumentNullException(nameof(requestMessage));

            var result = new RequestMessage();

            await CopyTargetToRequestMessage(result, requestMessage, cancellationToken);
            
            return result;
        }

        public async Task<ResponseMessage> FromResponseAsync(HttpResponseMessage responseMessage,
            CancellationToken cancellationToken)
        {
            if (responseMessage == null) throw new ArgumentNullException(nameof(responseMessage));

            var result = new ResponseMessage();

            await CopyTargetToResponseMessage(result, responseMessage, cancellationToken);

            return result;
        }

        public Task CopyRequestMessageToTarget(HttpRequestMessage target, RequestMessage requestMessage,
            CancellationToken cancellationToken)
        {
            target.Method = new HttpMethod(requestMessage.RequestMethod);

            target.RequestUri = requestMessage.Uri;
            
            if (requestMessage.Headers != null)
            {
                target.Headers.FillFromHeaderCollection(requestMessage.Headers);
            }

            if (requestMessage.Body != null)
            {
                target.Content = new StreamContent(requestMessage.Body);
            }

            return Task.CompletedTask;
        }

        public Task CopyResponseMessageToTarget(HttpResponseMessage target, ResponseMessage responseMessage,
            CancellationToken cancellationToken)
        {
            target.StatusCode = (HttpStatusCode) responseMessage.StatusCode;
                
            target.ReasonPhrase = responseMessage.StatusMessage;
            
            if (responseMessage.Headers != null)
            {
                target.Headers.FillFromHeaderCollection(responseMessage.Headers);
            }

            if (responseMessage.Body != null)
            {
                target.Content = new StreamContent(responseMessage.Body);
            }

            return Task.CompletedTask;
        }

        public async Task CopyTargetToRequestMessage(RequestMessage requestMessage, HttpRequestMessage target,
            CancellationToken cancellationToken)
        {
            requestMessage.ProtocolVersion = $"HTTP/{target.Version.ToString(2)}";

            requestMessage.RequestMethod = target.Method.Method;

            requestMessage.Uri = target.RequestUri;

            if (target.Headers != null)
            {
                requestMessage.Headers = target.Headers.ToHeaderCollection();
            }

            if (target.Content != null)
            {
                requestMessage.Body = await target.Content.ReadAsStreamAsync();
            }
        }

        public async Task CopyTargetToResponseMessage(ResponseMessage responseMessage, HttpResponseMessage target,
            CancellationToken cancellationToken)
        {
            responseMessage.StatusCode = (int)target.StatusCode;

            responseMessage.StatusMessage = target.ReasonPhrase;

            if (target.Headers != null)
            {
                responseMessage.Headers = target.Headers.ToHeaderCollection();
            }

            if (target.Content != null)
            {
                responseMessage.Body = await target.Content.ReadAsStreamAsync();
            }
        }
    }
}