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


        public HttpCookie this[string key]
        {
            get
            {
                if (Headers.CookieStrings == null)
                {
                    throw new KeyNotFoundException();
                }

                var hash = key.GetHashCode();
                if (_indexHash.TryGetValue(hash, out var index))
                {
                    var str = Headers.CookieStrings.ElementAt(index);
                    _parseCookieEntry(str, out var name, out _);
                    if (name == key)
                    {
                        return HttpCookie.Parse(str);
                    }
                }

                foreach (var str in Headers.CookieStrings)
                {
                    _parseCookieEntry(str, out var name, out _);
                    if (name == key)
                    {
                        return HttpCookie.Parse(str);
                    }
                }

                throw new KeyNotFoundException();
            }
            set
            {
                if (Headers.CookieStrings == null)
                {
                    throw new KeyNotFoundException();
                }

                var hash = key.GetHashCode();
                if (_indexHash.TryGetValue(hash, out var index))
                {
                    var str = Headers.CookieStrings.ElementAt(index);
                    _parseCookieEntry(str, out var name, out _);
                    if (name == key)
                    {
                        var cookies = Headers.CookieStrings.ToArray();
                        cookies[index] = value.ToString();
                        Headers.CookieStrings = cookies;
                        return;
                    }
                }

                index = 0;
                foreach (var str in Headers.CookieStrings)
                {
                    _parseCookieEntry(str, out var name, out _);
                    if (name == key)
                    {
                        var cookies = Headers.CookieStrings.ToArray();
                        cookies[index] = value.ToString();
                        Headers.CookieStrings = cookies;
                        return;
                    }

                    index++;
                }

                {
                    var cookies = Headers.CookieStrings.ToList();
                    cookies.Add(value.ToString());
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

        private void _parseCookieEntry(string cookieString, out string name, out string value)
        {
            var equalSign = cookieString.IndexOf('=');
            var semicolon = cookieString.IndexOf(';');

            if (semicolon < 0)
            {
                semicolon = cookieString.Length;
            }

            name = cookieString.Substring(0, equalSign);
            value = cookieString.Substring(equalSign + 1, semicolon - equalSign);
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
                    _parseCookieEntry(cookie, out var name, out _);
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

        public bool Contains(HttpCookie item) => TryGetValue(item.Name, out _);

        public bool ContainsKey(string key) => TryGetValue(key, out _);

        public bool TryGetValue(string key, out string value)
        {
            if (Headers.CookieStrings == null)
            {
                value = default;
                return false;
            }

            var hash = key.GetHashCode();
            if (_indexHash.TryGetValue(hash, out var index))
            {
                var str = Headers.CookieStrings.ElementAt(index);
                _parseCookieEntry(str, out var name, out value);
                if (name == key)
                {
                    return true;
                }
            }

            foreach (var str in Headers.CookieStrings)
            {
                _parseCookieEntry(str, out var name, out value);
                if (name == key)
                {
                    return true;
                }
            }

            value = default;
            return false;
        }

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

        public bool Remove(HttpCookie item)
        {
            if (Headers.CookieStrings == null)
            {
                throw new KeyNotFoundException();
            }

            var hash = item.Name.GetHashCode();
            if (_indexHash.TryGetValue(hash, out var index))
            {
                var str = Headers.CookieStrings.ElementAt(index);
                _parseCookieEntry(str, out var name, out _);
                if (name == item.Name)
                {
                    var cookies = Headers.CookieStrings.ToList();
                    cookies.RemoveAt(index);
                    Headers.CookieStrings = cookies;
                    return true;
                }
            }

            index = 0;
            foreach (var str in Headers.CookieStrings)
            {
                _parseCookieEntry(str, out var name, out _);
                if (name == item.Name)
                {
                    var cookies = Headers.CookieStrings.ToList();
                    cookies.RemoveAt(index);
                    Headers.CookieStrings = cookies;
                    return true;
                }

                index++;
            }

            return false;
        }

        public int Count => Headers.CookieStrings?.Count() ?? 0;
        public bool IsReadOnly => Headers.IsReadOnly;
    }
}