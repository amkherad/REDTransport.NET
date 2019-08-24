using System;
using System.Collections;
using System.Collections.Generic;

namespace REDTransport.NET.Collections
{
    public partial class KeyValuesCollection<TKey, TValue>
    {
        public class KeyCollection : ICollection<TKey>
        {
            public KeyValuesCollection<TKey, TValue> Parent { get; }
            
            public KeyCollection(KeyValuesCollection<TKey, TValue> parent)
            {
                Parent = parent;
            }

            public IEnumerator<TKey> GetEnumerator()
                => Parent._entries.Keys.GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator()
                => Parent._entries.Keys.GetEnumerator();

            public void Add(TKey item)
            {
                if (Parent._removeEmptyKeys)
                {
                    throw new InvalidOperationException();
                }

                ((ICollection<TKey>)Parent._entries.Keys).Add(item);
                Parent._orderedKeys.Add(item);
            }

            public void Clear()
            {
                Parent._entries.Clear();
                Parent._orderedKeys.Clear();
            }

            public bool Contains(TKey item)
            {
                return Parent.ContainsKey(item);
            }

            public void CopyTo(TKey[] array, int arrayIndex)
            {
                for (var i = arrayIndex; i < Parent._entries.Count; i++)
                {
                    array[i] = Parent._orderedKeys[i];
                }
            }

            public bool Remove(TKey item)
            {
                Parent._orderedKeys.Remove(item);
                return Parent._entries.Remove(item);
            }

            public int Count => Parent.KeyCount;
            public bool IsReadOnly => Parent.IsReadOnly;
        }
    }
}