using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Http.Internal;
using REDTransport.NET.Messages;
using REDTransport.NET.Server.AspNet.Http;

namespace REDTransport.NET.Server.AspNet.Pipeline
{
    public class RedHttpResponse : HttpResponse
    {
        private static readonly Func<IFeatureCollection, IHttpResponseFeature> NullResponseFeature =
            f => (IHttpResponseFeature) null;

        private static readonly Func<IFeatureCollection, IResponseCookiesFeature> NewResponseCookiesFeature =
            f => (IResponseCookiesFeature) new ResponseCookiesFeature(f);

        public override HttpContext HttpContext { get; }

        public ResponseMessage ResponseMessage { get; }

        private readonly HeaderCollectionWrapper _header;
        private readonly CookieCollectionWrapper _cookies;

        private FeatureReferences<FeatureInterfaces> _features;

        public RedHttpResponse(HttpContext httpContext, ResponseMessage responseMessage)
        {
            HttpContext = httpContext;
            ResponseMessage = responseMessage;
            _header = new HeaderCollectionWrapper(ResponseMessage.Headers);
            _cookies = new CookieCollectionWrapper(ResponseMessage.Headers.Cookies);
            _features = new FeatureReferences<FeatureInterfaces>(httpContext.Features);
        }

        public override IHeaderDictionary Headers => _header;

        public override int StatusCode
        {
            get => ResponseMessage.StatusCode;
            set => ResponseMessage.StatusCode = value;
        }

        public override Stream Body
        {
            get => ResponseMessage.Body;
            set => ResponseMessage.Body = value;
        }

        public override long? ContentLength
        {
            get => ResponseMessage.Headers.ContentLength;
            set => ResponseMessage.Headers.ContentLength = value;
        }

        public override string ContentType
        {
            get => ResponseMessage.Headers.ContentType;
            set => ResponseMessage.Headers.ContentType = value;
        }

        public override IResponseCookies Cookies => _cookies;


        public override bool HasStarted => this.HttpResponseFeature.HasStarted;


        public override void OnStarting(Func<object, Task> callback, object state)
        {
            if (callback == null)
                throw new ArgumentNullException(nameof(callback));
            this.HttpResponseFeature.OnStarting(callback, state);
        }

        public override void OnCompleted(Func<object, Task> callback, object state)
        {
            if (callback == null)
                throw new ArgumentNullException(nameof(callback));
            this.HttpResponseFeature.OnCompleted(callback, state);
        }

        public override void Redirect(string location, bool permanent)
        {
            ResponseMessage.StatusCode = !permanent ? 302 : 301;
            ResponseMessage.Headers.Location = location;
        }


        private IHttpResponseFeature HttpResponseFeature => this._features.Fetch(ref this._features.Cache.Response,
            NullResponseFeature);

        private IResponseCookiesFeature ResponseCookiesFeature => this._features.Fetch(ref this._features.Cache.Cookies,
            NewResponseCookiesFeature);

        private struct FeatureInterfaces
        {
            public IHttpResponseFeature Response;
            public IResponseCookiesFeature Cookies;
        }
    }
}