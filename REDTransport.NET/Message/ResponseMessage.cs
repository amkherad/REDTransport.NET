using System.Net.Http;

namespace REDTransport.NET.Message
{
    public class ResponseMessage
    {
        public HttpResponseMessage HttpResponseMessage { get; set; }

        public string CorrelationId
        {
            get { return ""; }
        }
    }
}