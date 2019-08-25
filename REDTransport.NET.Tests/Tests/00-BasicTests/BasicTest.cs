using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using REDTransport.NET.RESTClient;
using REDTransport.NET.Tests.Server;

namespace REDTransport.NET.Tests
{
    [TestFixture]
    public class BasicTest
    {
        [Test]
        public async Task Test1()
        {
            using (var server = new TestServerFixture())
            {
                var client = server.Client.ToTransporter();

                var res = await client.GetAsync(server.Client.Route("api/v1/test/testdata"), CancellationToken.None);
                
                var response = await client.HttpConverter.ToResponseAsync(res, CancellationToken.None);
                var responseText = await response.GetContentString();
                
                Console.WriteLine(responseText);
                
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    throw new Exception(await response.GetContentString());
                }

                await response.AssertJsonResponse(new
                {
                    id = 100001,
                    firstName = "Ali",
                    lastName = "Mousavi Kherad",
                    age = 19,
                });
            }
        }
    }
}