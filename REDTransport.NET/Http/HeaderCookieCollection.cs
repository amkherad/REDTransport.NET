using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace REDTransport.NET.Http
{
    public class HeaderCookieCollection : ICollection<HttpCookie>
    {
        public HeaderCollection Headers { get; }

        private Dictionary<string, HttpCookie> _cookies;
        private Dictionary<int, int> _indexHash;


        public HeaderCookieCollection(HeaderCollection headers)
        {
            Headers = headers;
            _cookies = new Dictionary<string, HttpCookie>();
            _indexHash = new Dictionary<int, int>();
        }

        public IEnumerator<HttpCookie> GetEnumerator()
        {
            if (Headers.CookieStrings == null)
            {
                return Enumerable.Empty<HttpCookie>().GetEnumerator();
            }

            return Headers.CookieStrings.Select(HttpCookie.Parse).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();


        public int IndexOf(string key) => IndexOf(key, out _, out _);

        public int IndexOf(string key, out string cookieString)
        {
            if (Headers.CookieStrings == null)
            {
                cookieString = default;
                return -1;
            }

            var hash = key.GetHashCode();
            if (_indexHash.TryGetValue(hash, out var index))
            {
                var str = Headers.CookieStrings.ElementAt(index);
                _parseCookieEntry(str, out var name, out _, out _);
                if (name == key)
                {
                    cookieString = str;
                    return index;
                }
            }

            index = 0;
            foreach (var str in Headers.CookieStrings)
            {
                _parseCookieEntry(str, out var name, out _, out _);
                if (name == key)
                {
                    cookieString = str;
                    return index;
                }

                index++;
            }

            cookieString = default;
            return -1;
        }

        public int IndexOf(string key, out string value, out string remaining)
        {
            if (Headers.CookieStrings == null)
            {
                value = default;
                remaining = default;
                return -1;
            }

            var hash = key.GetHashCode();
            if (_indexHash.TryGetValue(hash, out var index))
            {
                var str = Headers.CookieStrings.ElementAt(index);
                _parseCookieEntry(str, out var name, out value, out remaining);
                if (name == key)
                {
                    return index;
                }
            }

            index = 0;
            foreach (var str in Headers.CookieStrings)
            {
                _parseCookieEntry(str, out var name, out value, out remaining);
                if (name == key)
                {
                    return index;
                }

                index++;
            }

            value = default;
            remaining = default;
            return -1;
        }


        public HttpCookie this[string key]
        {
            get
            {
                var index = IndexOf(key, out var cookieString);

                if (index < 0)
                {
                    throw new KeyNotFoundException();
                }

                return HttpCookie.Parse(cookieString);
            }
            set
            {
                if (Headers.CookieStrings == null)
                {
                    throw new KeyNotFoundException();
                }

                var index = IndexOf(key, out _, out _);

                if (index < 0)
                {
                    var cookies = Headers.CookieStrings.ToList();
                    cookies.Add(value.ToString());
                    Headers.CookieStrings = cookies;
                }
                else
                {
                    var cookies = Headers.CookieStrings.ToArray();
                    cookies[index] = value.ToString();
                    Headers.CookieStrings = cookies;
                }
            }
        }

        public HttpCookie this[int index]
        {
            get
            {
                if (Headers.CookieStrings == null)
                {
                    throw new IndexOutOfRangeException();
                }

                var entry = Headers.CookieStrings.ElementAt(index);

                return HttpCookie.Parse(entry);
            }
            set
            {
                if (Headers.CookieStrings == null)
                {
                    throw new IndexOutOfRangeException();
                }

                var cookies = Headers.CookieStrings?.ToArray();
                cookies[index] = value.ToString();
                Headers.CookieStrings = cookies;
            }
        }

        private void _parseCookieEntry(
            string cookieString,
            out string name,
            out string value,
            out string remaining
        )
        {
            var equalSign = cookieString.IndexOf('=');
            var semicolon = cookieString.IndexOf(';');

            if (semicolon < 0)
            {
                semicolon = cookieString.Length;
            }

            name = cookieString.Substring(0, equalSign);
            value = cookieString.Substring(equalSign + 1, semicolon - equalSign);

            if (semicolon < cookieString.Length)
            {
                remaining = cookieString.Substring(semicolon + 1);
            }
            else
            {
                remaining = default;
            }
        }

        public IEnumerable<string> Keys
        {
            get
            {
                if (Headers.CookieStrings == null)
                {
                    yield break;
                }

                foreach (var cookie in Headers.CookieStrings)
                {
                    _parseCookieEntry(cookie, out var name, out _, out _);
                    yield return name;
                }
            }
        }

        public void Add(HttpCookie cookie)
        {
            var cookies = Headers.CookieStrings?.ToList() ?? new List<string>();
            cookies.Add(cookie.ToString());
            Headers.CookieStrings = cookies;
        }

        public void Add(string name, string value)
        {
            var cookies = Headers.CookieStrings?.ToList() ?? new List<string>();
            cookies.Add($"{name}={value}");
            Headers.CookieStrings = cookies;
        }

        public bool AddOrUpdate(HttpCookie cookie)
        {
            var index = IndexOf(cookie.Name);

            if (index < 0)
            {
                Add(cookie);
                return true;
            }

            this[index] = cookie;
            return false;
        }

        public void AddRange(IEnumerable<HttpCookie> newCookies)
        {
            var cookies = Headers.CookieStrings?.ToList() ?? new List<string>();
            foreach (var cookie in newCookies)
            {
                cookies.Add(cookie.ToString());
            }

            Headers.CookieStrings = cookies;
        }

        public void Clear()
        {
            Headers.CookieStrings = null;
        }

        public bool Contains(HttpCookie item) => TryGetValue(item.Name, out _, out _);

        public bool ContainsKey(string key) => TryGetValue(key, out _, out _);

        public bool TryGetValue(string key, out string value, out string remaining) =>
            IndexOf(key, out value, out remaining) >= 0;

        public void CopyTo(HttpCookie[] array, int arrayIndex)
        {
            if (Headers.CookieStrings == null)
            {
                throw new IndexOutOfRangeException();
            }

            var cookies = Headers.CookieStrings.ToArray();
            for (var i = arrayIndex; i < array.Length && i < cookies.Length; i++)
            {
                array[i] = HttpCookie.Parse(cookies[i]);
            }
        }

        public bool Remove(HttpCookie item) => Remove(item.Name);

        public bool Remove(string key)
        {
            if (Headers.CookieStrings == null)
            {
                throw new KeyNotFoundException();
            }

            var index = IndexOf(key);

            if (index < 0)
            {
                return false;
            }

            var cookies = Headers.CookieStrings.ToList();
            cookies.RemoveAt(index);
            Headers.CookieStrings = cookies;
            return true;
        }

        public int Count => Headers.CookieStrings?.Count() ?? 0;
        public bool IsReadOnly => Headers.IsReadOnly;
    }
}