using System;
using System.Runtime.Serialization;

namespace REDTransport.NET
{
    public class ProtocolException : Exception
    {
        public const string YieldCorrelationIdIsMissing = "CorrelationId header is required when response action is yield.";
        
        public ProtocolException()
        {
        }

        protected ProtocolException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public ProtocolException(string message)
            : base(message)
        {
        }

        public ProtocolException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}