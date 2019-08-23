namespace REDTransport.NET.Exceptions
{
    public class RedTransportFlowControlException : RedTransportException
    {
        public RedTransportFlowControlException()
            : base("FlowControlException")
        {
        }
    }
}