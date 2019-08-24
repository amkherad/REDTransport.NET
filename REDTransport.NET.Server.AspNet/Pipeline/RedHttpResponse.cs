using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using REDTransport.NET.Messages;

namespace REDTransport.NET.Server.AspNet.Pipeline
{
    public class RedHttpResponse : HttpResponse
    {
        public override HttpContext HttpContext { get; }
        
        public ResponseMessage ResponseMessage { get; }


        public RedHttpResponse(HttpContext httpContext, ResponseMessage responseMessage)
        {
            HttpContext = httpContext;
            ResponseMessage = responseMessage;
        }

        public override IHeaderDictionary Headers => 
        public override int StatusCode
        {
            get;
            set;
        }
        public override Stream Body { get; set; }
        public override long? ContentLength { get; set; }
        public override string ContentType { get; set; }
        public override IResponseCookies Cookies { get; }
        public override bool HasStarted { get; }


        public override void OnStarting(Func<object, Task> callback, object state)
        {
        }

        public override void OnCompleted(Func<object, Task> callback, object state)
        {
        }

        public override void Redirect(string location, bool permanent)
        {
        }
    }
}