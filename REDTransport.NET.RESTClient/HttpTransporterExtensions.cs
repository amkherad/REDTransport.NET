using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using REDTransport.NET.Http;
using REDTransport.NET.Messages;

namespace REDTransport.NET.RESTClient
{
    public static class HttpTransporterExtensions
    {
        public static async Task<ResponseMessage> SendAsync(
            this HttpTransporter httpTransporter,
            HttpMethod method,
            Uri requestUri,
            HttpRequestHeaders headers,
            HttpContent content,
            CancellationToken cancellationToken)
        {
            if (httpTransporter == null) throw new ArgumentNullException(nameof(httpTransporter));

            var message = new RequestMessage(
                httpTransporter.HttpProtocol,
                requestUri,
                method.ToString(),
                headers.ToHeaderCollection(),
                await content.ReadAsStreamAsync()
            );

            return await httpTransporter.SendAsync(message, cancellationToken);
        }

        public static Task<ResponseMessage> GetAsync(
            this HttpTransporter httpTransporter,
            Uri requestUri,
            CancellationToken cancellationToken)
        {
            if (httpTransporter == null) throw new ArgumentNullException(nameof(httpTransporter));

            var message = new RequestMessage(
                httpTransporter.HttpProtocol,
                requestUri,
                HttpMethod.Get.ToString(),
                new HeaderCollection(),
                null
            );

            return httpTransporter.SendAsync(message, cancellationToken);
        }

        public static Task<ResponseMessage> GetAsync(this HttpTransporter httpTransporter, string uri,
            CancellationToken cancellationToken) => GetAsync(httpTransporter, new Uri(uri), cancellationToken);

//        public static async Task<T> GetAsync<T>(
//            this HttpTransporter httpTransporter,
//            Uri requestUri,
//            CancellationToken cancellationToken)
//        {
//            if (httpTransporter == null) throw new ArgumentNullException(nameof(httpTransporter));
//
//            var response = await httpTransporter.GetAsync(requestUri, cancellationToken);
//
//            var jsonStream = await response.Content.ReadAsStreamAsync();
//
//            return await JsonSerializer.ReadAsync<T>(jsonStream, cancellationToken: cancellationToken);
//        }
    }
}