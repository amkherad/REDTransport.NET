using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using REDTransport.NET.RESTClient;

namespace REDTransport.NET
{
    public static class TransporterExtensions
    {
        public static Task<HttpResponseMessage> SendAsync(
            this Transporter transporter,
            HttpMethod method,
            Uri requestUri,
            HttpRequestHeaders headers,
            HttpContent content,
            CancellationToken cancellationToken)
        {
            if (transporter == null) throw new ArgumentNullException(nameof(transporter));

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

            return transporter.SendAsync(message, cancellationToken);
        }

        public static Task<HttpResponseMessage> GetAsync(
            this Transporter transporter,
            Uri requestUri,
            CancellationToken cancellationToken)
        {
            if (transporter == null) throw new ArgumentNullException(nameof(transporter));

            var message = new HttpRequestMessage(HttpMethod.Get, requestUri);

            return transporter.SendAsync(message, cancellationToken);
        }

        public static async Task<T> GetAsync<T>(
            this Transporter transporter,
            Uri requestUri,
            CancellationToken cancellationToken)
        {
            if (transporter == null) throw new ArgumentNullException(nameof(transporter));

            var response = await transporter.GetAsync(requestUri, cancellationToken);

            var jsonStream = await response.Content.ReadAsStreamAsync();

            return await JsonSerializer.ReadAsync<T>(jsonStream, cancellationToken: cancellationToken);
        }
    }
}