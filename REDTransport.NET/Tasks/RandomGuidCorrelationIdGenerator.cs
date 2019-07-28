using System;
using System.Net.Http;

namespace REDTransport.NET.Tasks
{
    public class RandomGuidCorrelationIdGenerator : ICorrelationIdGenerator
    {
        public string GenerateNewId(HttpRequestMessage message)
        {
            return Guid.NewGuid().ToString("D");
        }
    }
}