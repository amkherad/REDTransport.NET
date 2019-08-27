using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using REDTransport.NET.Messages;

namespace REDTransport.NET.Server.AspNet.Message
{
    public class MultipartMessageReaderWriter : IMessageReaderWriter
    {
        
        public string GetResponseContentTypeFromRequestContentType(string requestContentType)
        {
            if (requestContentType == null) throw new ArgumentNullException(nameof(requestContentType));
            
            requestContentType = requestContentType.ToLower();

            if (requestContentType == "application/json")
            {
                return "application/json";
            }

            if (requestContentType == "text/json")
            {
                return "text/json";
            }

            throw new InvalidOperationException();
        }
        
        public async Task WriteResponseMessageToStream(
            Stream stream,
            ResponseMessage message,
            CancellationToken cancellationToken
        )
        {
            throw new NotImplementedException();
            
//            if (stream == null) throw new ArgumentNullException(nameof(stream));
//            if (message == null) throw new ArgumentNullException(nameof(message));
//
//            using (var writer = new StreamWriter(stream))
//            {
//                await writer.WriteLineAsync("");
//            }
        }

        public Task WriteResponseMessageToStream(Stream stream, IAsyncEnumerable<ResponseMessage> messages, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}