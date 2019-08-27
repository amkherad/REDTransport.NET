using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using REDTransport.NET.Collections;

namespace REDTransport.NET.Http
{
    public class HeaderCollection : NameStringsCollection
    {
        public const string Rfc7231DateFormat = "ddd, dd MMM yyyy HH:mm:ss 'GMT'"; //Same As RFC1123
        public const string Rfc7231DateFormatToString = "R"; //R = RFC1123

        #region Http Header Names

        public const string AcceptHeaderName = "Accept";
        public const string AcceptCharsetHeaderName = "Accept-Charset";
        public const string AcceptEncodingHeaderName = "Accept-Encoding";
        public const string AcceptLanguageHeaderName = "Accept-Language";
        public const string AcceptDateTimeHeaderName = "Accept-Datetime";
        public const string AcceptControlRequestMethodHeaderName = "Accept-Control-Request-Method";
        public const string AcceptControlRequestHeadersHeaderName = "Accept-Control-Request-Headers";
        public const string AcceptRangesHeaderName = "Accept-Ranges";

        public const string AuthorizationHeaderName = "Authorization";

        public const string CacheControlHeaderName = "Cache-Control";

        public const string ConnectionHeaderName = "Connection";

        public const string RequestCookieHeaderName = "Cookie";
        public const string ResponseCookieHeaderName = "Set-Cookie";

        public const string ContentLengthHeaderName = "Content-Length";
        public const string ContentMd5HeaderName = "Content-MD5";
        public const string ContentRangeHeaderName = "Content-Range";
        public const string ContentTypeHeaderName = "Content-Type";

        public const string DateHeaderName = "Date";

        public const string ExpectHeaderName = "Expect";

        public const string ForwardedHeaderName = "Forwarded";

        public const string FromHeaderName = "From";

        public const string HostHeaderName = "Host";

        public const string IfMatchHeaderName = "If-Match";
        public const string IfModifiedSinceHeaderName = "If-Modified-Since";
        public const string IfNoneMatchHeaderName = "If-None-Match";
        public const string IfRangeHeaderName = "If-Range";
        public const string IfUnmodifiedSinceHeaderName = "If-Unmodified-Since";

        public const string LocationHeaderName = "Location";
        
        public const string MaxForwardsHeaderName = "Max-Forwards";

        public const string OriginHeaderName = "Origin";

        public const string PragmaHeaderName = "Pragma";

        public const string ProxyAuthorizationHeaderName = "Proxy-Authorization";

        public const string RangeHeaderName = "Range";

        public const string RefererHeaderName = "Referer";

        public const string TeHeaderName = "TE";

        public const string UserAgentHeaderName = "User-Agent";

        public const string UpgradeHeaderName = "Upgrade";

        public const string ViaHeaderName = "Via";

        public const string WarningHeaderName = "Warning";

        #endregion

        private readonly string _cookieHeaderName;
        private readonly HeaderCookieCollection _cookies;
        
        public HeaderCollection(HttpHeaderType httpHeaderType)
        {
            _cookies = new HeaderCookieCollection(this);
            _cookieHeaderName = httpHeaderType == HttpHeaderType.ResponseHeader
                ? ResponseCookieHeaderName
                : RequestCookieHeaderName;
        }

        #region Helper Methods

        public override string SingleOrDefault(string key)
        {
            var value = base.SingleOrDefault(key);

            if (value != null)
            {
                if (value.Contains(';'))
                {
                    var parts = value.Split(';');
                    value = parts[0];
                }
            }
            
            return value;
        }
        
        public override string Single(string key)
        {
            var value = base.Single(key);

            if (value != null)
            {
                if (value.Contains(';'))
                {
                    var parts = value.Split(';');
                    value = parts[0];
                }
            }
            
            return value;
        }

        protected void SetStringOrRemoveOnNull(string key, string value)
        {
            if (value == null)
            {
                RemoveKey(key);
            }
            else
            {
                Set(key, value);
            }
        }

        protected DateTime? GetDateTimeAsRfc7231(string key)
        {
            var dateStr = SingleOrDefault(key);
            return DateTime.TryParseExact(dateStr, Rfc7231DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out var result)
                ? (DateTime?)result
                : null;
        }

        protected void SetDateTimeAsRfc7231(string key, DateTime? value)
        {
            if (value == null)
            {
                RemoveKey(key);
            }
            else
            {
                Set(DateHeaderName, value?.ToUniversalTime().ToString(Rfc7231DateFormatToString));
            }
        }

        protected void SetDateTimeAsRfc7231OrRemoveOnNull(string key, DateTime? value)
        {
            if (value == null)
            {
                RemoveKey(key);
            }
            else
            {
                Set(DateHeaderName, value.Value.ToUniversalTime().ToString(Rfc7231DateFormatToString));
            }
        }

        protected long? GetLong(string key)
        {
            var longStr = SingleOrDefault(key);
            return longStr == null || !long.TryParse(longStr, out var result)
                ? null
                : (long?)result;
        }
        
        protected void SetLong(string key, long? value)
        {
            if (value == null)
            {
                RemoveKey(key);
            }
            else
            {
                Set(DateHeaderName, value.ToString());
            }
        }

        protected int? GetInt(string key)
        {
            var intStr = SingleOrDefault(key);
            return intStr == null || !int.TryParse(intStr, out var result)
                ? null
                : (int?)result;
        }
        
        protected void SetInt(string key, int? value)
        {
            if (value == null)
            {
                RemoveKey(key);
            }
            else
            {
                Set(DateHeaderName, value.ToString());
            }
        }
        #endregion
        


        #region Properties
        public string Accept
        {
            get => SingleOrDefault(AcceptHeaderName);
            set => SetStringOrRemoveOnNull(AcceptHeaderName, value);
        }

        public string AcceptCharset
        {
            get => SingleOrDefault(AcceptCharsetHeaderName);
            set => SetStringOrRemoveOnNull(AcceptCharsetHeaderName, value);
        }

        public string AcceptEncoding
        {
            get => SingleOrDefault(AcceptEncodingHeaderName);
            set => SetStringOrRemoveOnNull(AcceptEncodingHeaderName, value);
        }

        public string AcceptLanguage
        {
            get => SingleOrDefault(AcceptLanguageHeaderName);
            set => SetStringOrRemoveOnNull(AcceptLanguageHeaderName, value);
        }

        public DateTime? AcceptDateTime
        {
            get => GetDateTimeAsRfc7231(AcceptDateTimeHeaderName);
            set => SetDateTimeAsRfc7231OrRemoveOnNull(AcceptDateTimeHeaderName, value);
        }

        public string AcceptControlRequestMethod
        {
            get => SingleOrDefault(AcceptControlRequestMethodHeaderName);
            set => SetStringOrRemoveOnNull(AcceptControlRequestMethodHeaderName, value);
        }

        public string AcceptControlRequestHeaders
        {
            get => SingleOrDefault(AcceptControlRequestHeadersHeaderName);
            set => SetStringOrRemoveOnNull(AcceptControlRequestHeadersHeaderName, value);
        }

        public string AcceptRanges
        {
            get => SingleOrDefault(AcceptRangesHeaderName);
            set => SetStringOrRemoveOnNull(AcceptRangesHeaderName, value);
        }

        public string Authorization
        {
            get => SingleOrDefault(AuthorizationHeaderName);
            set => SetStringOrRemoveOnNull(AuthorizationHeaderName, value);
        }
        
        public string CacheControl
        {
            get => SingleOrDefault(CacheControlHeaderName);
            set => SetStringOrRemoveOnNull(CacheControlHeaderName, value);
        }
        
        public string Connection
        {
            get => SingleOrDefault(ConnectionHeaderName);
            set => SetStringOrRemoveOnNull(ConnectionHeaderName, value);
        }
        
        public IEnumerable<string> CookieStrings
        {
            get => Get(_cookieHeaderName);
            set => Set(_cookieHeaderName, value);
        }

        public HeaderCookieCollection Cookies => _cookies;

        public long? ContentLength
        {
            get => GetLong(ContentLengthHeaderName);
            set => SetLong(ContentLengthHeaderName, value);
        }

        public string ContentMd5
        {
            get => SingleOrDefault(ContentMd5HeaderName);
            set => SetStringOrRemoveOnNull(ContentMd5HeaderName, value);
        }

        public string ContentRange
        {
            get => SingleOrDefault(ContentRangeHeaderName);
            set => SetStringOrRemoveOnNull(ContentRangeHeaderName, value);
        }

        public string Encoding
        {
            get
            {
                var value = base.SingleOrDefault(ContentTypeHeaderName);

                if (value != null)
                {
                    if (value.Contains(';'))
                    {
                        var parts = value.Split(';');

                        return parts[1]; //Content-Type: application/json; utf8
                    }
                }

                return null;
            }
        }

        public string ContentType
        {
            get => SingleOrDefault(ContentTypeHeaderName);
            set => SetStringOrRemoveOnNull(ContentTypeHeaderName, value);
        }

        public DateTime? Date
        {
            get => GetDateTimeAsRfc7231(DateHeaderName);
            set => SetDateTimeAsRfc7231OrRemoveOnNull(DateHeaderName, value);
        }

        public string Expect
        {
            get => SingleOrDefault(ExpectHeaderName);
            set => SetStringOrRemoveOnNull(ExpectHeaderName, value);
        }

        public string Forwarded
        {
            get => SingleOrDefault(ForwardedHeaderName);
            set => SetStringOrRemoveOnNull(ForwardedHeaderName, value);
        }

        public string From
        {
            get => SingleOrDefault(FromHeaderName);
            set => SetStringOrRemoveOnNull(FromHeaderName, value);
        }

        public string Host
        {
            get => SingleOrDefault(HostHeaderName);
            set => SetStringOrRemoveOnNull(HostHeaderName, value);
        }

        public string IfMatch
        {
            get => SingleOrDefault(IfMatchHeaderName);
            set => SetStringOrRemoveOnNull(IfMatchHeaderName, value);
        }

        public string IfModifiedSince
        {
            get => SingleOrDefault(IfModifiedSinceHeaderName);
            set => SetStringOrRemoveOnNull(IfModifiedSinceHeaderName, value);
        }

        public string IfNoneMatch
        {
            get => SingleOrDefault(IfNoneMatchHeaderName);
            set => SetStringOrRemoveOnNull(IfNoneMatchHeaderName, value);
        }

        public string IfRange
        {
            get => SingleOrDefault(IfRangeHeaderName);
            set => SetStringOrRemoveOnNull(IfRangeHeaderName, value);
        }

        public string IfUnmodifiedSince
        {
            get => SingleOrDefault(IfUnmodifiedSinceHeaderName);
            set => SetStringOrRemoveOnNull(IfUnmodifiedSinceHeaderName, value);
        }

        public string Location
        {
            get => SingleOrDefault(LocationHeaderName);
            set => SetStringOrRemoveOnNull(LocationHeaderName, value);
        }

        public string MaxForwards
        {
            get => SingleOrDefault(MaxForwardsHeaderName);
            set => SetStringOrRemoveOnNull(MaxForwardsHeaderName, value);
        }

        public string Origin
        {
            get => SingleOrDefault(OriginHeaderName);
            set => SetStringOrRemoveOnNull(OriginHeaderName, value);
        }

        public string Pragma
        {
            get => SingleOrDefault(PragmaHeaderName);
            set => SetStringOrRemoveOnNull(PragmaHeaderName, value);
        }

        public string ProxyAuthorization
        {
            get => SingleOrDefault(ProxyAuthorizationHeaderName);
            set => SetStringOrRemoveOnNull(ProxyAuthorizationHeaderName, value);
        }

        public string  Range
        {
            get => SingleOrDefault(RangeHeaderName);
            set => SetStringOrRemoveOnNull(RangeHeaderName, value);
        }

        public string  Referer
        {
            get => SingleOrDefault(RefererHeaderName);
            set => SetStringOrRemoveOnNull(RefererHeaderName, value);
        }

        public string  TE
        {
            get => SingleOrDefault(TeHeaderName);
            set => SetStringOrRemoveOnNull(TeHeaderName, value);
        }

        public string  UserAgent
        {
            get => SingleOrDefault(UserAgentHeaderName);
            set => SetStringOrRemoveOnNull(UserAgentHeaderName, value);
        }

        public string  Upgrade
        {
            get => SingleOrDefault(UpgradeHeaderName);
            set => SetStringOrRemoveOnNull(UpgradeHeaderName, value);
        }

        public string  Via
        {
            get => SingleOrDefault(ViaHeaderName);
            set => SetStringOrRemoveOnNull(ViaHeaderName, value);
        }

        public string  Warning
        {
            get => SingleOrDefault(WarningHeaderName);
            set => SetStringOrRemoveOnNull(WarningHeaderName, value);
        }
        #endregion
    }
}