using System;
using System.Diagnostics;
using System.IO;
using REDTransport.NET.Http;

namespace REDTransport.NET.Messages
{
    [DebuggerDisplay("{StatusCode} \"{StatusMessage}\" HTTP/{Version}, Headers={Headers.Count}")]
    public class ResponseMessage
    {
        private HeaderCollection _headers;
        
        public int StatusCode { get; set; }
        
        public string StatusMessage { get; set; }
        
        public string Version { get; set; }
        
        //public string ProtocolVersion { get; set; }

        public HeaderCollection Headers
        {
            get => _headers;
            set => _headers = value ?? throw new ArgumentNullException(nameof(value));
        }
        
        public Stream Body { get; set; }


        public ResponseMessage()
        {
            Headers = new HeaderCollection(HttpHeaderType.ResponseHeader);
            StatusCode = 200;
            StatusMessage = "OK";
            Body = Stream.Null;
            Version = "1.1";
        }
        
        public ResponseMessage(
            int statusCode,
            string statusMessage,
            HeaderCollection headers,
            Stream body
        )
        {
            //if (body == null) throw new ArgumentNullException(nameof(body));

            StatusMessage = statusMessage ?? throw new ArgumentNullException(nameof(statusMessage));
            Headers = headers ?? throw new ArgumentNullException(nameof(headers));
            StatusCode = statusCode;
            Body = body;
        }
        

        public string CorrelationId
        {
            get => Headers.Single(ProtocolConstants.REDCorrelationIdHeaderName);
            set => Headers.Set(ProtocolConstants.REDCorrelationIdHeaderName, value);
        }
    }
}