using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.AspNetCore.Http;
using REDTransport.NET.Http;

namespace REDTransport.NET.Server.AspNet.Http
{
    public class CookieCollectionWrapper : IResponseCookies, IRequestCookieCollection
    {
        public HeaderCookieCollection Cookies { get; }

        public CookieCollectionWrapper(HeaderCookieCollection cookies)
        {
            Cookies = cookies;
        }


        public void Append(string key, string value)
        {
            Cookies.Add(key, value);
        }

        public void Append(string key, string value, CookieOptions options)
        {
            if (options == null)
            {
                Append(key, value);
                return;
            }


            var cookie = new HttpCookie(key, value);
            _fillFromCookieOptions(ref cookie, options);
            
            Cookies.Add(cookie);
        }

        public void Delete(string key)
        {
            Cookies.Remove(key);
        }

        public void Delete(string key, CookieOptions options)
        {
            var cookie = new HttpCookie(key, null);
            _fillFromCookieOptions(ref cookie, options);

            Cookies.Remove(cookie);
        }

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            using (var enumerator = Cookies.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    var current = enumerator.Current;
                    yield return new KeyValuePair<string, string>(current.Name, current.Value);
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public bool ContainsKey(string key) => Cookies.ContainsKey(key);

        public bool TryGetValue(string key, out string value)
        {
            return Cookies.TryGetValue(key, out value, out _);
        }

        public int Count => Cookies.Count;
        public ICollection<string> Keys => new ReadOnlyCollection<string>(Cookies.Keys.ToList());

        public string this[string key] => Cookies[key].Value;


        private void _fillFromCookieOptions(ref HttpCookie cookie, CookieOptions options)
        {
            cookie.Domain = options.Domain;
            cookie.Expires = options.Expires;
            cookie.Path = options.Path;
            cookie.Secure = options.Secure;
            cookie.HttpOnly = options.HttpOnly;
            
            switch (options.SameSite)
            {
                case SameSiteMode.None:
                    cookie.SameSite = null;
                    break;
                case SameSiteMode.Lax:
                    cookie.SameSite = HttpCookieSameSiteMode.Lax;
                    break;
                case SameSiteMode.Strict:
                    cookie.SameSite = HttpCookieSameSiteMode.Strict;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}