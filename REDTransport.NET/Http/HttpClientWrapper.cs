using System.Net.Http;

namespace REDTransport.NET.Http
{
    public class HttpClientWrapper : HttpClient
    {
        public HttpClientWrapper()
        {
        }

        public HttpClientWrapper(HttpMessageHandler handler)
        {
        }

        public HttpClientWrapper(HttpMessageHandler handler, bool disposeHandler)
        {
        }
    }
}