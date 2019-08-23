namespace REDTransport.NET.Exceptions
{
    /// <summary>
    /// A flow-control exception to yield current task.
    /// It's best to use other flow-control mechanisms and only use this exception when other options are not available. (because exceptions are slow in dotnet)  
    /// </summary>
    public class YieldTaskException : RedTransportFlowControlException
    {
        public YieldTaskException()
            : base()
        {
        }
    }
}