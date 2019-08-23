using System;
using System.Diagnostics;
using System.IO;
using REDTransport.NET.Http;

namespace REDTransport.NET.Messages
{
    [DebuggerDisplay("{StatusCode} \"{StatusMessage}\" {ProtocolVersion}, Headers={Headers.Count}")]
    public class ResponseMessage
    {
        public int StatusCode { get; set; }
        
        public string StatusMessage { get; set; }
        
        public string ProtocolVersion { get; set; }
        
        public HeaderCollection Headers { get; set; }
        
        public Stream Body { get; set; }


        public ResponseMessage()
        {
            
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