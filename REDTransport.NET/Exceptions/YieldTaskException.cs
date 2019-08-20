using System;
using System.Runtime.Serialization;

namespace REDTransport.NET.Exceptions
{
    public class YieldTaskException : RedTransportException
    {
        public YieldTaskException()
        {
        }

        public YieldTaskException(string message)
            : base(message)
        {
        }

        public YieldTaskException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected YieldTaskException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}