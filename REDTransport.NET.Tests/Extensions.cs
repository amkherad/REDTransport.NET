using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using REDTransport.NET.Http;
using REDTransport.NET.RESTClient;

namespace REDTransport.NET.Tests
{
    public static class Extensions
    {
        public static async Task<string> GetContentString(this HttpResponseMessage response)
        {
            return await response.Content.ReadAsStringAsync();
        }

        public static HttpTransporter ToTransporter(this HttpClient client)
        {
            if (client == null) throw new ArgumentNullException(nameof(client));
            
            return new HttpTransporter(client, new DefaultMessage2NetHttpConverter());
        }

        public static Uri Route(this HttpClient client, string route)
        {
            if (client == null) throw new ArgumentNullException(nameof(client));
            
            return new Uri($"{client.BaseAddress.Scheme}://{client.BaseAddress.Host}/{route}");
        }

        public static async Task AssertJsonResponse(this HttpResponseMessage response, object jsonObject)
        {
            var jsonString = await response.Content.ReadAsStringAsync();

            var actualJson = JObject.Parse(jsonString);
            var expectedJson = JObject.FromObject(jsonObject);

            Assert.IsTrue(JToken.DeepEquals(expectedJson, actualJson));
        }

        public static async Task AssertJsonResponse(this HttpResponseMessage response, object jsonObject, string message)
        {
            var jsonString = await response.Content.ReadAsStringAsync();

            var actualJson = JObject.Parse(jsonString);
            var expectedJson = JObject.FromObject(jsonObject);

            Assert.IsTrue(JToken.DeepEquals(expectedJson, actualJson));
        }
    }
}