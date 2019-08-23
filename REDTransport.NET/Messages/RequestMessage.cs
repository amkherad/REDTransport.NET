using System;
using System.IO;
using System.Linq;
using REDTransport.NET.Http;

namespace REDTransport.NET.Messages
{
    public class RequestMessage
    {
        public Uri Uri { get; set; }

        public string ProtocolVersion { get; set; }

        public string RequestMethod { get; set; }

        public HeaderCollection Headers { get; set; }

        public Stream Body { get; set; }


        public RequestMessage()
        {
        }

        public RequestMessage(
            string protocolVersion,
            Uri uri,
            string requestMethod,
            HeaderCollection headers,
            Stream body
        )
        {
            //if (body == null) throw new ArgumentNullException(nameof(body));

            ProtocolVersion = protocolVersion ?? throw new ArgumentNullException(nameof(protocolVersion));
            Uri = uri ?? throw new ArgumentNullException(nameof(uri));
            RequestMethod = requestMethod ?? throw new ArgumentNullException(nameof(requestMethod));
            Headers = headers ?? throw new ArgumentNullException(nameof(headers));
            Body = body;
        }

        public string CorrelationId
        {
            get => Headers.Single(ProtocolConstants.REDCorrelationIdHeaderName);
            set => Headers.Set(ProtocolConstants.REDCorrelationIdHeaderName, value);
        }
    }
}