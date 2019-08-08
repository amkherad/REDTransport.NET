using System;
using REDTransport.NET.Http;

namespace REDTransport.NET.Server.AspNet
{
    public class RedTransportMiddlewareConfiguration
    {
        public bool AllowRequestAggregation { get; set; }
        
        public IHttpConverter[] HttpConverters { get; set; }
        
        public Type[] Controllers { get; internal set; }


        public void MapControllers(params Type[] controllers)
        {
            Controllers = controllers;
        }
    }
}