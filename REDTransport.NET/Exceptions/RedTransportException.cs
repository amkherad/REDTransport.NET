using System;
using System.Runtime.Serialization;

namespace REDTransport.NET.Exceptions
{
    public class RedTransportException : Exception
    {
        public RedTransportException()
        {
        }

        public RedTransportException(string message)
            : base(message)
        {
        }

        public RedTransportException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected RedTransportException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}