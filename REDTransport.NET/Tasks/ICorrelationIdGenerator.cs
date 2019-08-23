using REDTransport.NET.Messages;

namespace REDTransport.NET.Tasks
{
    public interface ICorrelationIdGenerator
    {
        string GenerateNewId(RequestMessage message);
    }
}