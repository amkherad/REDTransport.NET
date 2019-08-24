using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using REDTransport.NET.Http;
using REDTransport.NET.Messages;
using REDTransport.NET.Server.AspNet.Helpers;

namespace REDTransport.NET.Server.AspNet.Message
{
    public class DefaultMessage2AspNetCoreHttpConverter : IRedTransportMessageConverter<HttpRequest, HttpResponse>
    {
        public bool IsRedRequest(HttpRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            if (request.Headers.TryGetValue(ProtocolConstants.REDProtocolVersionHeaderName, out var protocolVersion))
            {
                return true;
            }

            return false;
        }

        public bool IsRedResponse(HttpResponse request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            if (request.Headers.TryGetValue(ProtocolConstants.REDProtocolVersionHeaderName, out var protocolVersion))
            {
                return true;
            }

            return false;
        }

        public async Task<HttpRequest> ToRequestAsync(RequestMessage requestMessage, CancellationToken cancellationToken)
        {
            if (requestMessage == null) throw new ArgumentNullException(nameof(requestMessage));

            var result = new DefaultHttpRequest(null);

            await CopyRequestMessageToTarget(result, requestMessage, cancellationToken);
            
            return result;
        }

        public async Task<HttpResponse> ToResponseAsync(ResponseMessage responseMessage, CancellationToken cancellationToken)
        {
            if (responseMessage == null) throw new ArgumentNullException(nameof(responseMessage));

            var result = new DefaultHttpResponse(null);

            await CopyResponseMessageToTarget(result, responseMessage, cancellationToken);
            
            return result;
        }

        public async Task<RequestMessage> FromRequestAsync(HttpRequest requestMessage, CancellationToken cancellationToken)
        {
            if (requestMessage == null) throw new ArgumentNullException(nameof(requestMessage));
            
            var result = new RequestMessage();

            await CopyTargetToRequestMessage(result, requestMessage, cancellationToken);
            
            return result;
        }

        public async Task<ResponseMessage> FromResponseAsync(HttpResponse responseMessage,
            CancellationToken cancellationToken)
        {
            if (responseMessage == null) throw new ArgumentNullException(nameof(responseMessage));

            var result = new ResponseMessage();

            await CopyTargetToResponseMessage(result, responseMessage, cancellationToken);
            
            return result;
        }

        
        public Task CopyRequestMessageToTarget(HttpRequest target, RequestMessage requestMessage, CancellationToken cancellationToken)
        {
            if (requestMessage == null) throw new ArgumentNullException(nameof(requestMessage));

            target.Body = requestMessage.Body;

            if (requestMessage.Headers != null)
            {
                target.Headers.FillFromHeaderCollection(requestMessage.Headers);
            }

            target.Method = requestMessage.RequestMethod;

            target.Path = new PathString(requestMessage.Path);
            target.PathBase = new PathString(requestMessage.PathBase);
            target.Host = new HostString(requestMessage.Host);
            target.Scheme = requestMessage.Scheme;
            target.QueryString = new QueryString(requestMessage.QueryString);

            return Task.CompletedTask;
        }

        public Task CopyResponseMessageToTarget(HttpResponse target, ResponseMessage responseMessage,
            CancellationToken cancellationToken)
        {
            if (responseMessage == null) throw new ArgumentNullException(nameof(responseMessage));

            target.Body = responseMessage.Body;

            if (responseMessage.Headers != null)
            {
                target.Headers.FillFromHeaderCollection(responseMessage.Headers);
            }

            target.StatusCode = responseMessage.StatusCode;
            
            return Task.CompletedTask;
        }

        public Task CopyTargetToRequestMessage(RequestMessage requestMessage, HttpRequest target, CancellationToken cancellationToken)
        {
            if (requestMessage == null) throw new ArgumentNullException(nameof(requestMessage));

            requestMessage.Body = target.Body;

            if (target.Headers != null)
            {
                requestMessage.Headers = target.Headers.ToHeaderCollection();
            }

            requestMessage.ProtocolVersion = target.Protocol;

            requestMessage.RequestMethod = target.Method;
            
            requestMessage.Host = target.Host.ToString();
            requestMessage.Scheme = target.Scheme;
            requestMessage.PathBase = target.PathBase;
            requestMessage.Path = target.Path;
            requestMessage.QueryString = target.QueryString.ToUriComponent();
            
            return Task.CompletedTask;
        }

        public Task CopyTargetToResponseMessage(ResponseMessage responseMessage, HttpResponse target,
            CancellationToken cancellationToken)
        {
            if (responseMessage == null) throw new ArgumentNullException(nameof(responseMessage));

            responseMessage.Body = target.Body;

            if (target.Headers != null)
            {
                responseMessage.Headers = target.Headers.ToHeaderCollection();
            }

            responseMessage.StatusCode = target.StatusCode;

            return Task.CompletedTask;
        }
    }
}