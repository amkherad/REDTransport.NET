using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using REDTransport.NET.Http;
using REDTransport.NET.Messages;
using REDTransport.NET.Server.AspNet.Helpers;
using REDTransport.NET.Server.AspNet.Http;

namespace REDTransport.NET.Server.AspNet.Pipeline
{
    public class RedHttpRequest : HttpRequest
    {
        public override HttpContext HttpContext { get; }

        public RequestMessage RequestMessage { get; }


        private IQueryCollection _query;
        private readonly HeaderCollection _headers;
        private readonly IRequestCookieCollection _cookies;
        private readonly IFormFeature _formFeature;


        public RedHttpRequest(HttpContext httpContext, RequestMessage requestMessage)
        {
            HttpContext = httpContext ?? throw new ArgumentNullException(nameof(httpContext));
            RequestMessage = requestMessage ?? throw new ArgumentNullException(nameof(requestMessage));
            _headers = requestMessage.Headers ??= new HeaderCollection(HttpHeaderType.RequestHeader);
            _cookies = new RequestCookieCollectionWrapper(_headers.Cookies);
            _formFeature = httpContext.Features.Get<IFormFeature>();
        }


        public override string Method
        {
            get => RequestMessage.RequestMethod;
            set => RequestMessage.RequestMethod = value;
        }

        public override string Scheme
        {
            get => RequestMessage.Scheme;
            set => RequestMessage.Scheme = value;
        }

        public override bool IsHttps
        {
            get => string.Equals(Scheme, "https", StringComparison.OrdinalIgnoreCase);
            set
            {
                if (value != IsHttps)
                {
                    Scheme = value ? "https" : "http";
                }
            }
        }

        public override HostString Host
        {
            get => new HostString(RequestMessage.Host);
            set => RequestMessage.Host = value.ToUriComponent();
        }

        public override PathString PathBase
        {
            get => RequestMessage.PathBase;
            set => RequestMessage.PathBase = value;
        }

        public override PathString Path
        {
            get => RequestMessage.Path;
            set => RequestMessage.Path = value;
        }

        public override QueryString QueryString
        {
            get => new QueryString(RequestMessage.QueryString);
            set => RequestMessage.QueryString = value.ToUriComponent();
        }

        public override IQueryCollection Query
        {
            get => _query;
            set => _query = value;
        }

        public override string Protocol
        {
            get => RequestMessage.ProtocolVersion;
            set => RequestMessage.ProtocolVersion = value;
        }

        public override Stream Body
        {
            get => RequestMessage.Body;
            set => RequestMessage.Body = value;
        }

        public override IHeaderDictionary Headers => _headers.ToHeaderDictionary();

        public override IRequestCookieCollection Cookies
        {
            get => _cookies;
            set
            {
                _headers.Cookies.Clear();

                _headers.Cookies.AddRange(value.Select(
                    cookie => new HttpCookie(cookie.Key, cookie.Value)
                ));
            }
        }

        public override long? ContentLength
        {
            get => _headers.ContentLength;
            set => _headers.ContentLength = value;
        }

        public override string ContentType
        {
            get => _headers.ContentType;
            set => _headers.ContentType = value;
        }

        public override bool HasFormContentType => _formFeature.HasFormContentType;

        public override IFormCollection Form
        {
            get => _formFeature.ReadForm();
            set => _formFeature.Form = value;
        }

        public override Task<IFormCollection> ReadFormAsync(
            CancellationToken cancellationToken = default)
        {
            return _formFeature.ReadFormAsync(cancellationToken);
        }
    }
}