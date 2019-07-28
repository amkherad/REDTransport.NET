using System.Net.Http;

namespace REDTransport.NET.Tasks
{
    public interface ICorrelationIdGenerator
    {
        string GenerateNewId(HttpRequestMessage message);
    }
}