using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using REDTransport.NET.Messages;

namespace REDTransport.NET.Server.AspNet.Message
{
    public class JsonMessageReaderWriter : IMessageReaderWriter
    {
        public JsonSerializerOptions JsonSerializerOptions { get; }


        public JsonMessageReaderWriter(JsonSerializerOptions jsonSerializerOptions)
        {
            JsonSerializerOptions = jsonSerializerOptions;
        }
        
        public JsonMessageReaderWriter()
        {
            JsonSerializerOptions = new JsonSerializerOptions();
        }


        public async Task WriteResponseMessageToStream(
            Stream stream,
            ResponseMessage message,
            CancellationToken cancellationToken
        )
        {
            if (stream == null) throw new ArgumentNullException(nameof(stream));
            if (message == null) throw new ArgumentNullException(nameof(message));

            using (var writer = new StreamWriter(stream))
            {
                await WriteSingleResponseMessageToStream(stream, writer, message, cancellationToken);
            }
        }

        public async Task WriteResponseMessageToStream(
            Stream stream,
            IAsyncEnumerable<ResponseMessage> messages,
            CancellationToken cancellationToken
        )
        {
            await using (var writer = new StreamWriter(stream))
            {
                await writer.WriteAsync('[');
                try
                {
                    await foreach (var message in messages)
                    {
                        await WriteSingleResponseMessageToStream(stream, writer, message, cancellationToken);
                    }
                }
                catch (Exception ex)
                {
                    GC.KeepAlive(ex);
                }

                await writer.WriteAsync(']');
            }
        }


        private async Task WriteSingleResponseMessageToStream(
            Stream stream,
            StreamWriter writer,
            ResponseMessage message,
            CancellationToken cancellationToken
        )
        {
            if (message == null) throw new ArgumentNullException(nameof(message));

            await writer.WriteAsync('{');

            await writer.WriteAsync("\"StatusCode\":");
            await writer.WriteAsync(message.StatusCode.ToString());
            await writer.WriteAsync(',');

            await writer.WriteAsync("\"StatusMessage\":\"");
            await writer.WriteAsync(message.StatusMessage);
            await writer.WriteAsync("\",");

            if (message.Headers != null)
            {
                await writer.WriteAsync("\"Headers\":");
                //await JsonSerializer.SerializeAsync(stream, message.Headers, JsonSerializerOptions, cancellationToken);
                await writer.WriteAsync(',');
            }

            if (message.Body != null)
            {
                await writer.WriteAsync("\"Body\":");
                //await JsonSerializer.SerializeAsync(stream, message.Body, JsonSerializerOptions, cancellationToken);
                //writer.Write(',');
            }

            await writer.WriteAsync('}');
        }
    }
}