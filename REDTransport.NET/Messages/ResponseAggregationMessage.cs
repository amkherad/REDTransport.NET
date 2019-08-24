using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace REDTransport.NET.Messages
{
    public class ResponseAggregationMessage : ResponseMessage
    {
        internal protected ResponseAggregationMessage()
        {
            
        }
        
        public static Task<ResponseAggregationMessage> PackAsync(
            Uri uri,
            IEnumerable<ResponseMessage> responseMessages,
            CancellationToken cancellationToken
        )
        {
            return Task.FromResult(new ResponseAggregationMessage
            {
                //Uri = uri,
            });
        }

        public async IAsyncEnumerable<ResponseMessage> UnPackAsync(CancellationToken cancellationToken)
        {
            yield break;
        }
    }
}