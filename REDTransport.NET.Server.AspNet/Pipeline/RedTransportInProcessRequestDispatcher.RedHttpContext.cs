using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Http.Features;
using REDTransport.NET.Messages;

namespace REDTransport.NET.Server.AspNet.Pipeline
{
    public partial class RedTransportInProcessRequestDispatcher
    {
        public class RedHttpContext : HttpContext
        {
            public override IFeatureCollection Features { get; }
            
            public override HttpRequest Request { get; }
            public override HttpResponse Response { get; }
            
            public RequestMessage RequestMessage { get; }
            
            public RedHttpContext(FeatureCollection features, RequestMessage requestMessage)
            {
                Features = features;
                RequestMessage = requestMessage;
                
                Request = new RedHttpRequest(this, requestMessage);
                Response = new RedHttpResponse(this);
            }
            
            public override void Abort()
            {
                
            }

            public override ConnectionInfo Connection { get; }
            public override WebSocketManager WebSockets { get; }
            public override AuthenticationManager Authentication { get; }
            public override ClaimsPrincipal User { get; set; }
            public override IDictionary<object, object> Items { get; set; }
            public override IServiceProvider RequestServices { get; set; }
            public override CancellationToken RequestAborted { get; set; }
            public override string TraceIdentifier { get; set; }
            public override ISession Session { get; set; }
        }
    }
}