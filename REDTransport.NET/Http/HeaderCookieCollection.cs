using System;

namespace REDTransport.NET.Http
{
    public class HeaderCookieCollection : CookieCollection// ICollection<HttpCookie>
    {
        public HeaderCollection Headers { get; }
        
        
        public HeaderCookieCollection(HeaderCollection headers)
        {
            Headers = headers;
        }
        
    }
}