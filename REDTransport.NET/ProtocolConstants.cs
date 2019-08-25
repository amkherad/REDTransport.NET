namespace REDTransport.NET
{
    public class ProtocolConstants
    {
        public const string REDTransport = "Red Extensible Data Transport Protocol";
        public const string REDProtocolVersionHeaderName = "RED-ProtocolVersion"; //required
        public const string REDCorrelationIdHeaderName = "RED-CorrelationId"; //required if yield
        public const string REDResponseActionHeaderName = "RED-ResponseAction"; //required if yield
        public const string REDYieldTimeoutHeaderName = "RED-YieldTimeout"; //required if yield
        public const string REDRequestActionHeaderName = "RED-RequestAction"; //required if aggregated
    }
}