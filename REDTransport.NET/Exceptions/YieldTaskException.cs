using System;
using System.Runtime.Serialization;

namespace REDTransport.NET.Exceptions
{
    public class YieldTaskException : RedTransportException
    {
        public YieldTaskException(string messageId)
            : base(messageId)
        {
        }

        public YieldTaskException(string messageId, string message)
            : base(messageId, message)
        {
        }

        public YieldTaskException(string messageId, string message, Exception innerException)
            : base(messageId, message, innerException)
        {
        }

        protected YieldTaskException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}