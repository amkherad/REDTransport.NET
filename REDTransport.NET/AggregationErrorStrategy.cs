namespace REDTransport.NET
{
    public static class AggregationErrorStrategy
    {
        public const string FailNextQueue = "fail";
        public const string ContinueNextQueue = "continue";
        public const string UseDefaultSettings = "normal";
    }
}