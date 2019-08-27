using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Http.Authentication.Internal;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Http.Features.Authentication;
using Microsoft.AspNetCore.Http.Internal;
using REDTransport.NET.Messages;

namespace REDTransport.NET.Server.AspNet.Pipeline
{
    public partial class RedTransportInProcessRequestDispatcher
    {
        public class RedHttpContext : HttpContext
        {
            private static readonly Func<IFeatureCollection, IServiceProvidersFeature> NewServiceProvidersFeature =
                f => (IServiceProvidersFeature) new ServiceProvidersFeature();

            private static readonly Func<IFeatureCollection, IItemsFeature> NewItemsFeature =
                f => (IItemsFeature) new ItemsFeature();

            private static readonly Func<IFeatureCollection, IHttpAuthenticationFeature> NewHttpAuthenticationFeature =
                f => (IHttpAuthenticationFeature) new HttpAuthenticationFeature();

            private static readonly Func<IFeatureCollection, IHttpRequestLifetimeFeature> NewHttpRequestLifetimeFeature
                = f => (IHttpRequestLifetimeFeature) new HttpRequestLifetimeFeature();

            private static readonly Func<IFeatureCollection, ISessionFeature> NewSessionFeature =
                f => (ISessionFeature) new DefaultSessionFeature();

            private static readonly Func<IFeatureCollection, ISessionFeature> NullSessionFeature =
                f => (ISessionFeature) null;

            private static readonly Func<IFeatureCollection, IHttpRequestIdentifierFeature>
                NewHttpRequestIdentifierFeature =
                    f => (IHttpRequestIdentifierFeature) new HttpRequestIdentifierFeature();


            public override IFeatureCollection Features { get; }

            public override HttpRequest Request { get; }
            public override HttpResponse Response { get; }

            public RequestMessage RequestMessage { get; }

            private ConnectionInfo _connectionInfo;
            private WebSocketManager _webSocketManager;
            private AuthenticationManager _authenticationManager;
#nullable enable
            private ClaimsPrincipal? _user;
#nullable disable

            private FeatureReferences<FeatureInterfaces> _features;


            public RedHttpContext(FeatureCollection features, RequestMessage requestMessage, ResponseMessage responseMessage)
            {
                Features = features;
                RequestMessage = requestMessage;

                this._features = new FeatureReferences<FeatureInterfaces>(features);

                //DefaultHttpResponse
                Request = new RedHttpRequest(this, requestMessage);
                Response = new RedHttpResponse(this, responseMessage);
            }

            public override void Abort()
            {
            }

            public override ConnectionInfo Connection
            {
                get
                {
                    if (_connectionInfo != null)
                    {
                        return _connectionInfo;
                    }

                    _connectionInfo = new DefaultConnectionInfo(Features);
                    return _connectionInfo;
                }
            }

            public override WebSocketManager WebSockets
            {
                get
                {
                    if (_webSocketManager != null)
                    {
                        return _webSocketManager;
                    }

                    _webSocketManager = new DefaultWebSocketManager(Features);
                    return _webSocketManager;
                }
            }

            public override AuthenticationManager Authentication
            {
                get
                {
                    if (_authenticationManager != null)
                    {
                        return _authenticationManager;
                    }

                    _authenticationManager = new DefaultAuthenticationManager(this);

                    return _authenticationManager;
                }
            }


            public override ClaimsPrincipal User
            {
                get
                {
                    ClaimsPrincipal claimsPrincipal = this.HttpAuthenticationFeature.User;
                    if (claimsPrincipal == null)
                    {
                        claimsPrincipal = new ClaimsPrincipal((IIdentity) new ClaimsIdentity());
                        this.HttpAuthenticationFeature.User = claimsPrincipal;
                    }

                    return claimsPrincipal;
                }
                set { this.HttpAuthenticationFeature.User = value; }
            }

            public override IDictionary<object, object> Items
            {
                get => this.ItemsFeature.Items;
                set => this.ItemsFeature.Items = value;
            }

            public override IServiceProvider RequestServices
            {
                get => this.ServiceProvidersFeature.RequestServices;
                set => this.ServiceProvidersFeature.RequestServices = value;
            }

            public override CancellationToken RequestAborted
            {
                get => this.LifetimeFeature.RequestAborted;
                set => this.LifetimeFeature.RequestAborted = value;
            }

            public override string TraceIdentifier
            {
                get => this.RequestIdentifierFeature.TraceIdentifier;
                set => this.RequestIdentifierFeature.TraceIdentifier = value;
            }

            public override ISession Session
            {
                get
                {
                    ISessionFeature sessionFeatureOrNull = this.SessionFeatureOrNull;
                    if (sessionFeatureOrNull == null)
                        throw new InvalidOperationException(
                            "Session has not been configured for this application or request.");
                    return sessionFeatureOrNull.Session;
                }
                set { this.SessionFeature.Session = value; }
            }

            private IItemsFeature ItemsFeature =>
                this._features.Fetch(ref this._features.Cache.Items, NewItemsFeature);

            private IServiceProvidersFeature ServiceProvidersFeature =>
                this._features.Fetch(ref this._features.Cache.ServiceProviders, NewServiceProvidersFeature);

            private IHttpAuthenticationFeature HttpAuthenticationFeature =>
                this._features.Fetch(ref this._features.Cache.Authentication, NewHttpAuthenticationFeature);

            private IHttpRequestIdentifierFeature RequestIdentifierFeature =>
                this._features.Fetch(ref this._features.Cache.RequestIdentifier, NewHttpRequestIdentifierFeature);

            private IHttpRequestLifetimeFeature LifetimeFeature =>
                this._features.Fetch(ref this._features.Cache.Lifetime, NewHttpRequestLifetimeFeature);

            private ISessionFeature SessionFeature =>
                this._features.Fetch(ref this._features.Cache.Session, NewSessionFeature);

            private ISessionFeature SessionFeatureOrNull =>
                this._features.Fetch(ref this._features.Cache.Session, NullSessionFeature);

            private struct FeatureInterfaces
            {
                public IItemsFeature Items;
                public IServiceProvidersFeature ServiceProviders;
                public IHttpAuthenticationFeature Authentication;
                public IHttpRequestLifetimeFeature Lifetime;
                public ISessionFeature Session;
                public IHttpRequestIdentifierFeature RequestIdentifier;
            }
        }
    }
}