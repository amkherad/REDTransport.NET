using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading;
using REDTransport.NET.Exceptions;
using REDTransport.NET.Http;

namespace REDTransport.NET.Message
{
    public class RequestAggregationMessage : RequestMessage
    {
        IAsyncEnumerable<RequestMessage> UnpackAsync(IServiceProvider serviceProvider, CancellationToken cancellationToken)
        {
            if (serviceProvider == null) throw new ArgumentNullException(nameof(serviceProvider));
            
            if (Headers == null)
            {
                throw new RedTransportProtocolException("HeaderIsNull", "Header is required.");
            }

            if (Body == null)
            {
                throw new RedTransportProtocolException("BodyIsNull", "Body is required.");
            }

            var contentType = Headers.ContentType ?? "multipart/mixed";

            if (contentType.StartsWith("multipart/"))
            {
                return UnpackMultipartRequestBodyAsync(serviceProvider, Body, contentType, cancellationToken);
            }
            else if (contentType == "application/json" || contentType == "text/json")
            {
                return UnpackJsonRequestBodyAsync(serviceProvider, Body, cancellationToken);
            }
            else
            {
                throw new RedTransportProtocolException("UnknownContentType", "Unknown content type.");
            }
        }

        public static async IAsyncEnumerable<RequestMessage> UnpackMultipartRequestBodyAsync(
            IServiceProvider serviceProvider,
            Stream body,
            string contentType,
            CancellationToken cancellationToken
        )
        {

            yield break;
        }
        
        public static async IAsyncEnumerable<RequestMessage> UnpackJsonRequestBodyAsync(
            IServiceProvider serviceProvider,
            Stream body,
            CancellationToken cancellationToken
        )
        {
            var jsonSerializerOptions = serviceProvider.GetService(typeof(JsonSerializerOptions)) as JsonSerializerOptions;
            if (jsonSerializerOptions == null)
            {
                throw new RedTransportException("JsonConverterIsNull", "JsonConverter is null.");
            }

            var document = JsonDocument.Parse(body, new JsonDocumentOptions
            {
                AllowTrailingCommas = true,
                CommentHandling = JsonCommentHandling.Allow,
                MaxDepth = 100
            });

            var rootRequests = document.RootElement.EnumerateArray();

            foreach (var request in rootRequests)
            {
                var requestObject = request.EnumerateObject();

                string requestMethod = null;
                string requestUri = null;
                HeaderCollection requestHeaders = null;
                Stream requestBody = null;
                
                foreach (var objProperty in requestObject)
                {
                    switch (objProperty.Name.ToLower())
                    {
                        case "method":
                        {
                            requestMethod = objProperty.Value.GetString();
                            break;
                        }
                        case "uri":
                        {
                            requestUri = objProperty.Value.GetString();
                            break;
                        }
                        case "headers":
                        {
                            //requestMethod = objProperty.Value.GetString();
                            break;
                        }
                        case "body":
                        {
                            //requestMethod = objProperty.Value.GetString();
                            break;
                        }
                        default:
                        {
                            throw new RedTransportException("UnknownJsonRequestKey");
                        }
                    }
                }

                if (string.IsNullOrWhiteSpace(requestMethod))
                {
                    throw new RedTransportException("UnknownJsonRequestMethod");
                }

                if (string.IsNullOrWhiteSpace(requestUri))
                {
                    throw new RedTransportException("UnknownJsonRequestUri");
                }

                yield return new RequestMessage
                {
                    Uri = new Uri(requestUri),
                    Headers = requestHeaders,
                    Body = requestBody
                };
            }
        }
    }
}