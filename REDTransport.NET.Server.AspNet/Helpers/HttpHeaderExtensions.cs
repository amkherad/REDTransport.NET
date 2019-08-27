using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using REDTransport.NET.Http;

namespace REDTransport.NET.Server.AspNet.Helpers
{
    public static class HttpHeaderExtensions
    {
        public static HeaderCollection ToHeaderCollection(this IHeaderDictionary headers)
        {
            if (headers == null) throw new ArgumentNullException(nameof(headers));

            HttpHeaderType headerType;
            if (headers.ContainsKey(HeaderCollection.ResponseCookieHeaderName))
            {
                headerType = HttpHeaderType.ResponseHeader;
            }
            else
            {
                headerType = HttpHeaderType.RequestHeader;
            }

            var hc = new HeaderCollection(headerType);

            foreach (var kv in headers)
            {
                hc.Add(kv.Key, kv.Value.ToArray());
            }

            return hc;
        }

        public static IHeaderDictionary ToHeaderDictionary(this HeaderCollection headers)
        {
            if (headers == null) throw new ArgumentNullException(nameof(headers));

            var result = new HeaderDictionary();

            if (headers.Any)
            {
                foreach (var key in headers.Keys)
                {
                    var values = headers.Get(key);
                    result.Add(key, new StringValues(values.ToArray()));
                }
            }

            return result;
        }

        public static IDictionary<string, IEnumerable<string>> ToDictionary(this HeaderCollection headers)
        {
            if (headers == null) throw new ArgumentNullException(nameof(headers));

            var result = new Dictionary<string, IEnumerable<string>>();

            if (headers.Any)
            {
                foreach (var key in headers.Keys)
                {
                    var values = headers.Get(key);
                    result.Add(key, values.ToArray());
                }
            }

            return result;
        }

        public static void FillFromHeaderCollection(this IHeaderDictionary headers, HeaderCollection targetHeaders)
        {
            if (headers == null) throw new ArgumentNullException(nameof(headers));
            if (targetHeaders == null) throw new ArgumentNullException(nameof(targetHeaders));

            if (targetHeaders.Any)
            {
                foreach (var kv in targetHeaders)
                {
                    headers.Add(kv.Key, kv.Value);
                }
            }
        }


        public static bool IsHeaderFlagPresented(this HeaderCollection headers, string headerName, string flagName)
        {
            if (headers.TryGetValues(headerName, out var allValues))
            {
                if (allValues != null)
                {
                    foreach (var value in allValues)
                    {
                        if (value.Contains(flagName))
                        {
                            var parts = value.Split(';');

                            if (parts.Any(p => string.Equals(flagName, p.Trim(), StringComparison.OrdinalIgnoreCase)))
                            {
                                return true;
                            }
                        }
                    }
                }
            }

            return false;
        }
    }
}