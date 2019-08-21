using System.Collections.Generic;

namespace REDTransport.NET.Http
{
    public class CookieCollection : List<HttpCookie>, ICollection<HttpCookie>
    {
        public CookieCollection()
        {
        }
    }
}