using System;
using System.Runtime.Serialization;

namespace REDTransport.NET.Exceptions
{
    public class RedTransportProtocolException : RedTransportException
    {
        public RedTransportProtocolException(string messageId)
            : base(messageId)
        {
        }

        public RedTransportProtocolException(string messageId, string message)
            : base(messageId, message)
        {
        }

        public RedTransportProtocolException(string messageId, string message, Exception innerException)
            : base(messageId, message, innerException)
        {
        }

        protected RedTransportProtocolException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}