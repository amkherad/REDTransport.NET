using System;
using System.Collections;
using System.Collections.Generic;

namespace REDTransport.NET.Collections
{
    public partial class KeyValuesCollection<TKey, TValue>
    {
        public class EnumerableValueCollection : ValueCollection<IEnumerable<TValue>>
        {
            public EnumerableValueCollection(KeyValuesCollection<TKey, TValue> parent) : base(parent)
            {
            }

            public override bool Contains(IEnumerable<TValue> item) => Parent.Contains(item);
        }
        
        public class ValueCollection : ValueCollection<TValue>
        {
            public ValueCollection(KeyValuesCollection<TKey, TValue> parent) : base(parent)
            {
            }

            public override bool Contains(TValue item) => Parent.ContainsValue(item);
        }

        public abstract class ValueCollection<TValue1> : ICollection<TValue1>
        {
            public KeyValuesCollection<TKey, TValue> Parent { get; }


            public ValueCollection(KeyValuesCollection<TKey, TValue> parent)
            {
                Parent = parent;
            }

            public IEnumerator<TValue1> GetEnumerator() => throw new NotImplementedException();

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

            public void Add(TValue1 item)
            {
                throw new InvalidOperationException();
            }

            public void Clear()
            {
                Parent.Clear();
            }

            public abstract bool Contains(TValue1 item); // => Parent.Contains(item);

            public void CopyTo(TValue1[] array, int arrayIndex)
            {
                throw new NotImplementedException();
            }

            public bool Remove(TValue1 item)
            {
                throw new NotImplementedException();
            }

            public int Count => Parent.Count;
            public bool IsReadOnly => Parent.IsReadOnly;
        }
    }
}