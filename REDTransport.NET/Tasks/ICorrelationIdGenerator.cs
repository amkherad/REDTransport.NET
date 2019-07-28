using REDTransport.NET.Message;

namespace REDTransport.NET.Tasks
{
    public interface ICorrelationIdGenerator
    {
        string GenerateNewId(RequestMessage message);
    }
}