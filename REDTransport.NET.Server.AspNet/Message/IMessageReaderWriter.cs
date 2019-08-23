using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using REDTransport.NET.Messages;

namespace REDTransport.NET.Server.AspNet.Message
{
    public interface IMessageReaderWriter
    {
        Task WriteResponseMessageToStream(
            Stream stream,
            ResponseMessage message,
            CancellationToken cancellationToken
        );
        
        Task WriteResponseMessageToStream(
            Stream stream,
            IAsyncEnumerable<ResponseMessage> messages,
            CancellationToken cancellationToken
        );
    }
}