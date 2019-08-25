using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
            var dI = arrayIndex;
            foreach (var header in Headers)
            {
                if (dI >= array.Length)
                {
                    return;
                }

                array[dI] = new KeyValuePair<string, StringValues>(header.Key, header.Value);
            }
        }

        public bool Remove(KeyValuePair<string, StringValues> item)
        {
            return Headers.Remove(item.Key, item.Value.ToList()) > 0;
        }

        public int Count => Headers.Count;
        public bool IsReadOnly => Headers.IsReadOnly;
        public void Add(string key, StringValues values)
        {
            Headers.Add(key, (IEnumerable<string>) values);
        }

        public bool ContainsKey(string key) => Headers.ContainsKey(key);

        public bool Remove(string key) => Headers.Remove(key);

        public bool TryGetValue(string key, out StringValues value)
        {
            var result = Headers.TryGetValues(key, out var values);

            if (result)
            {
                value = new StringValues(values.ToArray());                
            }
            
            return result;
        }

        public StringValues this[string key]
        {
            get => new StringValues(Headers[key].ToArray());
            set => Headers[key] = value.ToArray();
        }

        public long? ContentLength
        {
            get => Headers.ContentLength;
            set => Headers.ContentLength = value;
        }

        public ICollection<string> Keys => new ReadOnlyCollection<string>(Headers.Keys.ToArray());

        public ICollection<StringValues> Values =>
            new ReadOnlyCollection<StringValues>(Headers.Values.Select(e => new StringValues(e.ToArray())).ToList());
    }
}