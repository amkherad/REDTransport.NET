using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;

namespace REDTransport.NET.Http
{
    [DebuggerDisplay("{ToString()}")]
    public struct HttpCookie
    {
        public string Name { get; }
        public string Value { get; set; }
        
        public string StringValue { get; set; }
        
        public string Path { get; set; }
        public string Domain { get; set; }

        public HttpCookieSameSiteMode? SameSite { get; set; }

        public DateTimeOffset? Expires { get; set; }
        
        
        /// <summary>
        /// A secure cookie is only sent to the server with an encrypted request over the HTTPS protocol. Even with Secure, sensitive information should never be stored in cookies, as they are inherently insecure and this flag can't offer real protection. Starting with Chrome 52 and Firefox 52, insecure sites (http:) can't set cookies with the Secure directive.
        /// </summary>
        public bool Secure { get; set; }

        /// <summary>
        /// To help mitigate cross-site scripting (XSS) attacks, HttpOnly cookies are inaccessible to JavaScript's Document.cookie API; they are only sent to the server. For example, cookies that persist server-side sessions don't need to be available to JavaScript, and the HttpOnly flag should be set.
        /// </summary>
        public bool HttpOnly { get; set; }


        public bool IsSecureOnlyCookie => Name.StartsWith("__Secure-");
        public bool IsHostOnlyCookie => Name.StartsWith("__Host-");


        public HttpCookie(string name, string value)
        {
            Name = name;
            Value = value;
            Secure = false;
            HttpOnly = false;

            StringValue = null;
            Expires = null;
            Domain = null;
            Path = null;
            SameSite = null;
        }

        public string[] Values
        {
            get { return StringValue.Split(';'); }
            set { StringValue = string.Join(";", value); }
        }

        public override string ToString()
        {
            var attributes = new List<string>();

            if (Secure)
            {
                attributes.Add("Secure");
            }

            if (HttpOnly)
            {
                attributes.Add("HttpOnly");
            }

            if (Domain != null)
            {
                attributes.Add($"Domain={Domain}");
            }

            if (Expires != null)
            {
                attributes.Add($"Expires={Expires.Value.ToString(HeaderCollection.Rfc7231DateFormatToString)}");
            }

            if (SameSite != null)
            {
                attributes.Add($"SameSite={SameSite}");
            }

            if (attributes.Count > 0)
            {
                return $"{Name}={Value}; {string.Join("; ", attributes)}";
            }

            return $"{Name}={Value}";
        }

        public void FillFromString(string cookieString)
        {
            if (cookieString == null) throw new ArgumentNullException(nameof(cookieString));

            const string Cookie = "cookie:";
            const string SetCookie = "set-cookie:";

            cookieString = cookieString.Trim();
            if (
                cookieString.StartsWith(Cookie, StringComparison.OrdinalIgnoreCase) ||
                cookieString.StartsWith(SetCookie, StringComparison.OrdinalIgnoreCase)
            )
            {
                cookieString = cookieString.Substring(SetCookie.Length);
            }

            Value = cookieString;

            var parts = cookieString.Split(';');
            foreach (var part in parts)
            {
                var eqIndex = part.IndexOf('=');
                if (eqIndex == -1)
                {
                    switch (part.ToLower())
                    {
                        case "secure":
                        {
                            Secure = true;
                            break;
                        }
                        case "httponly":
                        {
                            HttpOnly = true;
                            break;
                        }
                    }
                }
                else
                {
                    var prop = part.Substring(0, eqIndex);
                    var value = part.Substring(eqIndex + 1);
                    switch (prop.ToLower())
                    {
                        case "expires":
                        case "expire":
                        {
                            DateTime expiration;
                            if (DateTime.TryParseExact(value, HeaderCollection.Rfc7231DateFormat,
                                CultureInfo.CurrentCulture, DateTimeStyles.AssumeUniversal, out expiration))
                            {
                                Expires = expiration;
                            }

                            break;
                        }
                        case "path":
                        {
                            Path = value;
                            break;
                        }
                        case "domain":
                        {
                            Domain = value;
                            break;
                        }
                        case "__host-id":
                        {
                            Domain = value;
                            break;
                        }
                        case "secure":
                        {
                            value = value.ToLower();
                            Secure = value == "secure" || value == "true" || value == "1";
                            break;
                        }
                        case "httponly":
                        {
                            value = value.ToLower();
                            HttpOnly = value == "httponly" || value == "true" || value == "1" || value == "http-only";
                            break;
                        }
                    }
                }
            }
        }

        public static HttpCookie Parse(string cookieString)
        {
            var cookie = new HttpCookie();
            cookie.FillFromString(cookieString);
            return cookie;
        }
    }
}