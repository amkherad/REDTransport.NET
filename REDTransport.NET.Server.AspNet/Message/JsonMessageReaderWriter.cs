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
            using (var writer = new StreamWriter(stream))
            {
                writer.Write('[');
                await foreach (var message in messages)
                {
                    await WriteSingleResponseMessageToStream(stream, writer, message, cancellationToken);
                }
                writer.Write(']');
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

            writer.Write('{');

            writer.Write("\"StatusCode\":");
            writer.Write(message.StatusCode);
            writer.Write(',');

            writer.Write("\"StatusMessage\":\"");
            writer.Write(message.StatusMessage);
            writer.Write("\",");

            writer.Write("\"Headers\":");
            await JsonSerializer.SerializeAsync(stream, message.Headers, JsonSerializerOptions, cancellationToken);
            writer.Write(',');

            writer.Write("\"Body\":");
            await JsonSerializer.SerializeAsync(stream, message.Body, JsonSerializerOptions, cancellationToken);
            //writer.Write(',');

            writer.Write('}');
        }
    }
}