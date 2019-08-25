using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using REDTransport.NET.Exceptions;
using REDTransport.NET.Http;

namespace REDTransport.NET.Messages
{
    public class RequestAggregationMessage : RequestMessage
    {
        public RequestAggregationMessage()
        {
        }

        public static Task<RequestAggregationMessage> PackAsync(IEnumerable<RequestMessage> subMessages,
            CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async IAsyncEnumerable<RequestMessage> UnpackAsync( /*IServiceProvider serviceProvider, */
            CancellationToken cancellationToken)
        {
            //if (serviceProvider == null) throw new ArgumentNullException(nameof(serviceProvider));

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
                await foreach(var item in UnpackMultipartRequestBodyAsync(Body, contentType, cancellationToken))
                    yield return item;
            }
            else if (contentType == "application/json" || contentType == "text/json")
            {
                await foreach(var item in UnpackJsonRequestBodyAsync(Body, cancellationToken))
                    yield return item;
            }
            else
            {
                throw new RedTransportUnknownContentTypeException();
            }
        }

        public static IAsyncEnumerable<RequestMessage> UnpackMultipartRequestBodyAsync(
            Stream body,
            string contentType,
            CancellationToken cancellationToken
        )
        {
            throw new NotImplementedException();
        }

        public static async IAsyncEnumerable<RequestMessage> UnpackJsonRequestBodyAsync(
            Stream body,
            CancellationToken cancellationToken
        )
        {
            var document = await JsonDocument.ParseAsync(body, new JsonDocumentOptions
            {
                AllowTrailingCommas = true,
                //CommentHandling = JsonCommentHandling.Allow,
                MaxDepth = 100
            }, cancellationToken);

            var rootRequests = document.RootElement.EnumerateArray();

            foreach (var request in rootRequests)
            {
                var requestObject = request.EnumerateObject();

                string requestProtocolVersion = "1.1";
                string requestMethod = null;
                string requestUri = null;
                HeaderCollection requestHeaders = null;
                Stream requestBody = null;

                foreach (var objProperty in requestObject)
                {
                    switch (objProperty.Name.ToLower())
                    {
                        case "protocol-version":
                        {
                            requestProtocolVersion = objProperty.Value.GetString();
                            break;
                        }

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
                            requestHeaders = new HeaderCollection(HttpHeaderType.RequestHeader);
                            foreach (var header in objProperty.Value.EnumerateObject())
                            {
                                switch (header.Value.ValueKind)
                                {
                                    case JsonValueKind.Array:
                                        requestHeaders.Add(header.Name,
                                            header.Value.EnumerateArray().Cast<string>().ToList());
                                        break;
                                    case JsonValueKind.String:
                                        requestHeaders.Add(header.Name, header.Value.GetString());
                                        break;
                                    case JsonValueKind.Undefined:
                                        break;
                                    default:
                                        throw new NotSupportedException();
                                }
                            }

                            break;
                        }

                        case "body":
                        {
                            switch (objProperty.Value.ValueKind)
                            {
                                case JsonValueKind.Null:
                                case JsonValueKind.Undefined:
                                    break;
                                case JsonValueKind.Object:
                                    throw new NotImplementedException();
                                    break;
                                case JsonValueKind.Array:
                                    throw new NotImplementedException();
                                    break;
                                case JsonValueKind.String:
                                    using (var transform = new FromBase64Transform())
                                    {
                                        requestBody = new CryptoStream(
                                            new MemoryStream(Encoding.ASCII.GetBytes(objProperty.Value.GetString())),
                                            transform,
                                            CryptoStreamMode.Read
                                        );
                                    }

                                    break;
                                case JsonValueKind.Number:
                                    throw new NotImplementedException();
                                    break;
                                case JsonValueKind.True:
                                    throw new NotImplementedException();
                                    break;
                                case JsonValueKind.False:
                                    throw new NotImplementedException();
                                    break;
                                default:
                                    throw new ArgumentOutOfRangeException();
                            }

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
                    requestMethod = "GET";
                    //throw new RedTransportException("UnknownJsonRequestMethod");
                }

                if (string.IsNullOrWhiteSpace(requestUri))
                {
                    throw new RedTransportException("UnknownJsonRequestUri");
                }

                if (System.Uri.CheckHostName(requestUri) == UriHostNameType.Unknown)
                {
                    string path;
                    string queryString;

                    var questionMarkIndex = requestUri.IndexOf('?');
                    if (questionMarkIndex >= 0)
                    {
                        path = requestUri.Substring(0, questionMarkIndex);
                        queryString = requestMethod.Substring(questionMarkIndex + 1);
                    }
                    else
                    {
                        path = requestUri;
                        queryString = string.Empty;
                    }

                    if (!path.StartsWith('/'))
                    {
                        path = '/' + path;
                    }
                    
                    var scheme = "http";
                    var host = string.Empty;
                    var pathBase = string.Empty;
                    var rawTarget = string.Empty;
                    
                    yield return new RequestMessage(
                        requestProtocolVersion,
                        scheme,
                        host,
                        pathBase,
                        path,
                        queryString,
                        rawTarget,
                        requestMethod,
                        requestHeaders,
                        requestBody
                    );
                }
                else
                {
                    var uri = new Uri(requestUri);
                    yield return new RequestMessage(
                        requestProtocolVersion,
                        uri,
                        requestMethod,
                        requestHeaders,
                        requestBody
                    );
                }
                
            }
        }
    }
}