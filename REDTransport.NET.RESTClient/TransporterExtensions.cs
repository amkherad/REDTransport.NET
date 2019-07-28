using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using REDTransport.NET.Message;

namespace REDTransport.NET.RESTClient
{
    public static class HttpTransporterExtensions
    {
        public static Task<ResponseMessage> SendAsync(
            this HttpTransporter httpTransporter,
            HttpMethod method,
            Uri requestUri,
            HttpRequestHeaders headers,
            HttpContent content,
            CancellationToken cancellationToken)
        {
            if (httpTransporter == null) throw new ArgumentNullException(nameof(httpTransporter));

            var message = new HttpRequestMessage(method, requestUri)
            {
                Content = content
            };

            foreach (var httpRequestHeader in headers)
            {
                if (!message.Headers.TryAddWithoutValidation(httpRequestHeader.Key, httpRequestHeader.Value))
                {
                    throw new InvalidOperationException();
                }
            }

            return httpTransporter.SendAsync(message, cancellationToken);
        }

        public static Task<ResponseMessage> GetAsync(
            this HttpTransporter httpTransporter,
            Uri requestUri,
            CancellationToken cancellationToken)
        {
            if (httpTransporter == null) throw new ArgumentNullException(nameof(httpTransporter));

            var message = new HttpRequestMessage(HttpMethod.Get, requestUri);

            return httpTransporter.SendAsync(message, cancellationToken);
        }

        public static async Task<T> GetAsync<T>(
            this HttpTransporter httpTransporter,
            Uri requestUri,
            CancellationToken cancellationToken)
        {
            if (httpTransporter == null) throw new ArgumentNullException(nameof(httpTransporter));

            var response = await httpTransporter.GetAsync(requestUri, cancellationToken);

            var jsonStream = await response.Content.ReadAsStreamAsync();

            return await JsonSerializer.ReadAsync<T>(jsonStream, cancellationToken: cancellationToken);
        }
    }
}