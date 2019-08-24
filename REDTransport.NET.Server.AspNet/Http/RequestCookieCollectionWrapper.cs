using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.AspNetCore.Http;
using REDTransport.NET.Http;

namespace REDTransport.NET.Server.AspNet.Http
{
    public class RequestCookieCollectionWrapper : IRequestCookieCollection
    {
        public HeaderCookieCollection CookieCollection { get; }

        public RequestCookieCollectionWrapper(HeaderCookieCollection cookieCollection)
        {
            CookieCollection = cookieCollection;
        }

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            throw new System.NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public bool ContainsKey(string key) => CookieCollection.ContainsKey(key);

        public bool TryGetValue(string key, out string value) => CookieCollection.TryGetValue(key, out value);

        public int Count => CookieCollection.Count;
        public ICollection<string> Keys => new ReadOnlyCollection<string>(CookieCollection.Keys.ToList());

        public string this[string key] => CookieCollection[key].Value;


        public class Enumerator : IEnumerator<KeyValuePair<string, string>>
        {
            public IEnumerator<string> CookieStringEnumerator { get; }

            private string _currentCookie;

            public Enumerator(IEnumerator<string> cookieStringEnumerator)
            {
                CookieStringEnumerator = cookieStringEnumerator;
            }


            public bool MoveNext()
            {
                var result = CookieStringEnumerator.MoveNext();
                if (result)
                {
                    _currentCookie = CookieStringEnumerator.Current;
                }

                return result;
            }

            public void Reset() => CookieStringEnumerator.Reset();

            public KeyValuePair<string, string> Current
            {
                get
                {
                    var cookie = HttpCookie.Parse(_currentCookie);

                    return new KeyValuePair<string, string>(cookie.Name, cookie.Value);
                }
            }

            object IEnumerator.Current => Current;

            public void Dispose() => CookieStringEnumerator.Dispose();
        }
    }
}