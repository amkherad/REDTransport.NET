using System;
using System.IO;
using REDTransport.NET.Http;

namespace REDTransport.NET.Message
{
    public class RequestMessage
    {
        public Uri Uri { get; set; }
        
        public HeaderCollection Headers { get; set; }
        
        public Stream Body { get; set; }

        public RequestMessage()
        {
        }
    }
}