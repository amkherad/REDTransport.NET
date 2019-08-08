using System;

namespace REDTransport.NET.Server.AspNet.Crud
{
    public static class RedTransportCrudExtensions
    {
        public static void AddCrud(
            this RedTransportMiddlewareConfiguration configuration,
            Action<RedTransportCrudContext> contextCreator
        )
        {
            var context = new RedTransportCrudContext();
            contextCreator(context);

        }
    }
}