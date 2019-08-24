using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using REDTransport.NET.Http;

namespace REDTransport.NET.Server.AspNet.Http
{
    public class HeaderCollectionWrapper : IHeaderDictionary
    {
        public HeaderCollection Headers { get; }
        
        public HeaderCollectionWrapper(HeaderCollection headers)
        {
            Headers = headers;
        }

        
        public IEnumerator<KeyValuePair<string, StringValues>> GetEnumerator()
        {
            yield break;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(KeyValuePair<string, StringValues> item)
        {
            Headers.Add(item.Key, item.Value.ToList());
        }

        public void Clear()
        {
            Headers.Clear();
        }

        public bool Contains(KeyValuePair<string, StringValues> item)
        {
            return Headers.Contains(new KeyValuePair<string, string>(item.Key, item.Value));
        }

        public void CopyTo(KeyValuePair<string, StringValues>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public bool Remove(KeyValuePair<string, StringValues> item)
        {
            return Headers.Remove(item.Key, item.Value.ToList()) > 0;
        }

        public int Count { get; }
        public bool IsReadOnly { get; }
        public void Add(string key, StringValues value)
        {
            throw new System.NotImplementedException();
        }

        public bool ContainsKey(string key)
        {
            throw new System.NotImplementedException();
        }

        public bool Remove(string key)
        {
            throw new System.NotImplementedException();
        }

        public bool TryGetValue(string key, out StringValues value)
        {
            throw new System.NotImplementedException();
        }

        public StringValues this[string key]
        {
            get => throw new System.NotImplementedException();
            set => throw new System.NotImplementedException();
        }

        public long? ContentLength { get; set; }
        public ICollection<string> Keys { get; }
        public ICollection<StringValues> Values { get; }
    }
}