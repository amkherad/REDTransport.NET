using System;
using System.Text.Json;
using NUnit.Framework;
using REDTransport.NET.Http;
using REDTransport.NET.Server.AspNet.Helpers;

namespace REDTransport.NET.Tests
{
    [TestFixture]
    public class KeyValuesCollectionTests
    {
        [Test]
        public void TestHeaderCollection1()
        {
            var headers = new HeaderCollection(HttpHeaderType.RequestHeader);

            headers.Add("Content-Type", "application/json");
            headers.Add("Content-Length", "application/json");
            headers.Cookies.Add("Token", "Test1");
            headers.Cookies.Add("Token", "Test2");

            foreach (var header in headers)
            {
                Console.WriteLine(header.ToString());
            }

            var col = headers.ToDictionary();

            var str = JsonSerializer.Serialize(col);
            Console.WriteLine(str);
        }
    }
}