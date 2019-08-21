using System;

namespace REDTransport.NET.Message
{
    public class ResponseAggregationMessage : ResponseMessage
    {
        public static ResponseAggregationMessage Pack(
            Uri uri,
            params ResponseMessage[] responseMessages
        )
        {
            return new ResponseAggregationMessage
            {
                //Uri = uri,
            };
        }
    }
}