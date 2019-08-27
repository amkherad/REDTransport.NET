using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Internal;
using REDTransport.NET.Http;
using REDTransport.NET.Messages;

namespace REDTransport.NET.Server.AspNet.Message
{
    public class SystemTextJsonMessageReaderWriter : IMessageReaderWriter
    {
        public JsonSerializerOptions JsonSerializerOptions { get; }


        public SystemTextJsonMessageReaderWriter(JsonSerializerOptions jsonSerializerOptions)
        {
            JsonSerializerOptions = jsonSerializerOptions;
        }

        public SystemTextJsonMessageReaderWriter()
        {
            JsonSerializerOptions = new JsonSerializerOptions();
        }


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
                    var isFirst = true;
                    await foreach (var message in messages)
                    {
                        if (!isFirst)
                        {
                            await writer.WriteAsync(',');
                        }

                        isFirst = false;

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

            await writer.WriteAsync("{\"StatusCode\":");
            await writer.WriteAsync(message.StatusCode.ToString());

            await writer.WriteAsync(",\"StatusMessage\":\"");

            await writer.WriteAsync(message.StatusMessage);

            await writer.WriteAsync("\"");

            if (message.Headers != null && message.Headers.Any)
            {
                await writer.WriteAsync(",\"Headers\":");
                await SerializerHeaders(writer, message.Headers, cancellationToken);
            }

            if (message.Body != null)
            {
                message.Body.Position = 0;
                
                await writer.WriteAsync(",\"Body\":");
                
                await writer.FlushAsync();
                using (var stream1 = new MemoryStream())
                using (var reader = new StreamReader(message.Body))
                {
                    if (message.Headers != null)
                    {
                        var contentType = message.Headers.ContentType;
                        if (contentType == "application/json" || contentType == "text/json")
                        {
                            var input = await reader.ReadToEndAsync();
                            await writer.WriteAsync(input);
                        }
                    }
                    else
                    {
                        
                    }

                    //await JsonSerializer.SerializeAsync(stream, input, JsonSerializerOptions, cancellationToken);
                    //await stream1.CopyToAsync(stream, cancellationToken);
                }

                

//
//                await message.Body.CopyToAsync(stream);

                //writer.Write(',');
            }

            await writer.WriteAsync('}');
        }

        private async Task SerializerHeaders(
            StreamWriter writer,
            HeaderCollection headers,
            CancellationToken cancellationToken
        )
        {
            await writer.WriteAsync('{');
            var isFirst = true;
            foreach (var key in headers.Keys)
            {
                if (isFirst)
                {
                    await writer.WriteAsync($"\"{key}\":");
                    isFirst = false;
                }
                else
                {
                    await writer.WriteAsync($",\"{key}\":");
                }

                string jsonText;
                var value = headers[key].ToList();
                if (value.Count == 1)
                {
                    jsonText = JsonSerializer.Serialize(value[0], JsonSerializerOptions);
                }
                else
                {
                    jsonText = JsonSerializer.Serialize(value, JsonSerializerOptions);
                }

                await writer.WriteAsync(jsonText);
            }

            await writer.WriteAsync('}');
        }
    }
}