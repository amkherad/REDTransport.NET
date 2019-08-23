using System;
using System.Runtime.Serialization;

namespace REDTransport.NET.Exceptions
{
    public class RedTransportUnknownContentTypeException : RedTransportException
    {
        public RedTransportUnknownContentTypeException()
            : base("UnknownContentType")
        {
        }
        
        public RedTransportUnknownContentTypeException(string messageId)
            : base(messageId)
        {
        }

        public RedTransportUnknownContentTypeException(string messageId, string message)
            : base(messageId, message)
        {
        }

        public RedTransportUnknownContentTypeException(string messageId, string message, Exception innerException)
            : base(messageId, message, innerException)
        {
        }

        protected RedTransportUnknownContentTypeException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}