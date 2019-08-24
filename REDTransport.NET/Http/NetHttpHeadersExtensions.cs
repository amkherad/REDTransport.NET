using System;
using System.Linq;
using System.Net.Http.Headers;

namespace REDTransport.NET.Http
{
    public static class NetHttpHeadersExtensions
    {
//        public static HttpRequestHeaders ToHttpRequestHeaders(this HeaderCollection headers)
//        {
//            if (headers == null) throw new ArgumentNullException(nameof(headers));
//            
//            var rh = HttpRequestHeaders;
//        }

        public static HeaderCollection ToHeaderCollection(this HttpRequestHeaders headers)
        {
            if (headers == null) throw new ArgumentNullException(nameof(headers));

            var hc = new HeaderCollection(HttpHeaderType.RequestHeader);
            
            hc.AddRange(headers.ToList());

            return hc;
        }
        
        public static HeaderCollection ToHeaderCollection(this HttpResponseHeaders headers)
        {
            if (headers == null) throw new ArgumentNullException(nameof(headers));

            var hc = new HeaderCollection(HttpHeaderType.ResponseHeader);
            
            hc.AddRange(headers.ToList());

            return hc;
        }

        public static void FillFromHeaderCollection(this HttpRequestHeaders headers, HeaderCollection targetHeaders)
        {
            if (headers == null) throw new ArgumentNullException(nameof(headers));
            if (targetHeaders == null) throw new ArgumentNullException(nameof(targetHeaders));

            foreach (var kv in targetHeaders)
            {
                headers.Add(kv.Key, kv.Value);
            }
        }

        public static void FillFromHeaderCollection(this HttpResponseHeaders headers, HeaderCollection targetHeaders)
        {
            if (headers == null) throw new ArgumentNullException(nameof(headers));
            if (targetHeaders == null) throw new ArgumentNullException(nameof(targetHeaders));

            foreach (var kv in targetHeaders)
            {
                headers.Add(kv.Key, kv.Value);
            }
        }
    }
}