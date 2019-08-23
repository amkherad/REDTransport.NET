using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;

namespace REDTransport.NET.Tests.Server
{
    public class DefaultTestServer : WebApplicationFactory<DefaultStartup>
    {
        public DefaultTestServer()
        {
            
        }

        protected override IWebHostBuilder CreateWebHostBuilder()
        {
            return WebHost.CreateDefaultBuilder<DefaultStartup>(new string[0]);
        }

        protected override TestServer CreateServer(IWebHostBuilder builder)
        {
            var server = base.CreateServer(builder);

            return server;
        }
    }
}