using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using REDTransport.NET.Messages;
using REDTransport.NET.Server.AspNet.Helpers;

namespace REDTransport.NET.Server.AspNet.Pipeline
{
    public partial class RedTransportInProcessRequestDispatcher
    {
        public class RequestFeature : IHttpRequestFeature
        {
            public RequestMessage RequestMessage { get; }

#nullable enable
            private string? _protocol;
            private string? _scheme;
            private string? _method;
            private string? _pathBase;
            private string? _path;
            private string? _queryString;
            private string? _rawTarget;
            private IHeaderDictionary? _headers;
            private Stream? _body;
#nullable disable


            public RequestFeature(RequestMessage requestMessage)
            {
                RequestMessage = requestMessage;
            }


            public string Protocol
            {
                get => _protocol ??= RequestMessage.ProtocolVersion;
                set => _protocol = value;
            }

            public string Scheme
            {
                get => _scheme ??= RequestMessage.Uri.Scheme;
                set => _scheme = value;
            }

            public string Method
            {
                get => _method ??= RequestMessage.RequestMethod;
                set => _method = value;
            }

            public string PathBase
            {
                get => _pathBase ??= RequestMessage.PathBase;
                set => _pathBase = value;
            }

            public string Path
            {
                get => _path ??= RequestMessage.Path;
                set => _path = value;
            }

            public string QueryString
            {
                get => _queryString ??= RequestMessage.QueryString;
                set => _queryString = value;
            }

            public string RawTarget
            {
                get => _rawTarget ??= RequestMessage.RawTarget;
                set => _rawTarget = value;
            }

            public IHeaderDictionary Headers
            {
                get => _headers ??= RequestMessage.Headers.ToHeaderDictionary();
                set => _headers = value;
            }

            public Stream Body
            {
                get => _body ??= RequestMessage.Body;
                set => _body = value;
            }
        }
    }
}