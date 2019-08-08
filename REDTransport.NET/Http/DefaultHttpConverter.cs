using System.Net.Http;
using REDTransport.NET.Message;

namespace REDTransport.NET.Http
{
    public class DefaultHttpConverter : IHttpConverter
    {
        public HttpRequestMessage ToHttpRequestMessage(RequestMessage requestMessage)
        {
            return null;
        }
        
        public HttpResponseMessage ToHttpResponseMessage(ResponseMessage responseMessage)
        {
            return null;
        }
        
        
        public RequestMessage FromHttpRequestMessage(HttpRequestMessage requestMessage)
        {
            return null;
        }
        
        public ResponseMessage FromHttpResponseMessage(HttpResponseMessage responseMessage)
        {
            return null;
        }
    }
}