using System;
using System.Runtime.Serialization;

namespace REDTransport.NET.Exceptions
{
    public class RedTransportException : Exception
    {
        public string MessageId { get; }
        
        public RedTransportException(string messageId)
        {
            MessageId = messageId;
        }

        public RedTransportException(string messageId, string message)
            : base(message)
        {
            MessageId = messageId;
        }

        public RedTransportException(string messageId, string message, Exception innerException)
            : base(message, innerException)
        {
            MessageId = messageId;
        }

        protected RedTransportException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}