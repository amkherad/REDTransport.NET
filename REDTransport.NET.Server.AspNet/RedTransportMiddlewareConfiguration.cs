using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using REDTransport.NET.Exceptions;
using REDTransport.NET.Server.AspNet.Pipeline;

namespace REDTransport.NET.Server.AspNet
{
    [DebuggerDisplay("{RequestDispatcherStrategy}:{InProcessScopeMode}")]
    public class RedTransportMiddlewareConfiguration
    {
        internal ConcurrentDictionary<string, RedTransportEndpointConfiguration> Endpoints { get; }

        public bool AllowRequestAggregation { get; set; }

        public Type[] Controllers { get; internal set; }

        public RequestDispatcherStrategy RequestDispatcherStrategy { get; set; }

        public RedTransportInProcessScopeMode InProcessScopeMode { get; set; }


        public RedTransportMiddlewareConfiguration()
        {
            RequestDispatcherStrategy = RequestDispatcherStrategy.HttpChannel;
            InProcessScopeMode = RedTransportInProcessScopeMode.UsePerPipelineScope;

            Endpoints = new ConcurrentDictionary<string, RedTransportEndpointConfiguration>();
        }


        public void MapControllers(params Type[] controllers)
        {
            Controllers = controllers;
        }

        public void AddEndpoint(string name, Action<RedTransportEndpointConfiguration> endpointConfig)
        {
            var config = new RedTransportEndpointConfiguration();

            endpointConfig(config);

            if (!Endpoints.TryAdd(name, config))
            {
                throw new RedTransportException("EndpointConcurrencyException");
            }
        }
    }
}