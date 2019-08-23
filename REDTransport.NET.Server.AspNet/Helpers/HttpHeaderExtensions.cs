using System;
using Microsoft.AspNetCore.Http;
using REDTransport.NET.Http;

namespace REDTransport.NET.Server.AspNet.Helpers
{
    public static class HttpHeaderExtensions
    {
        public static HeaderCollection ToHeaderCollection(this IHeaderDictionary headers)
        {
            if (headers == null) throw new ArgumentNullException(nameof(headers));

            var hc = new HeaderCollection();

            foreach (var kv in headers)
            {
                hc.Add(kv.Key, kv.Value.ToArray());
            }

            return hc;
        }

        public static void FillFromHeaderCollection(this IHeaderDictionary headers, HeaderCollection targetHeaders)
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