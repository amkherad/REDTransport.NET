using System;
using REDTransport.NET.Message;

namespace REDTransport.NET.Tasks
{
    public class RandomGuidCorrelationIdGenerator : ICorrelationIdGenerator
    {
        public string GenerateNewId(RequestMessage message)
        {
            return Guid.NewGuid().ToString("D");
        }
    }
}