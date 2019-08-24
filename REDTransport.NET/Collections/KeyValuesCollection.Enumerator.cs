using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace REDTransport.NET.Collections
{
    public partial class KeyValuesCollection<TKey, TValue>
    {
        protected class Enumerator : IEnumerator<KeyValuePair<TKey, TValue>>
        {
            private readonly KeyValuesCollection<TKey, TValue> _parent;

            private int _entriesCount;
            private int _currentIndex;
            private int _currentSubIndex;
            //private TKey _currentKey;


            public Enumerator(KeyValuesCollection<TKey, TValue> collection)
            {
                _parent = collection;

                _entriesCount = _parent._entries.Count;
                _currentIndex = 0;
                _currentSubIndex = -1;
            }

            public void Dispose()
            {
                //_parent = null;
                _entriesCount = _parent._entries.Count;
                _currentIndex = 0;
                _currentSubIndex = -1;
            }

            public bool MoveNext()
            {
                if (_parent._entries.Count != _entriesCount)
                {
                    throw new System.Exception("A key was added to collection while enumerating.");
                }
                
                if (_parent._entries.Count == 0 || _parent._entries.Count <= _currentIndex)
                {
                    return false;
                }

                ++_currentSubIndex;

                var col = _parent.GetEntryByIndex(_currentIndex);
                var count = col.Count;
                while (count == 0 || count <= _currentSubIndex)
                {
                    _currentSubIndex = 0;
                    ++_currentIndex;
                    if (_parent._entries.Count <= _currentIndex)
                    {
                        return false;
                    }
                }

                return true;
            }

            public void Reset()
            {
                _entriesCount = _parent._entries.Count;
                _currentIndex = 0;
                _currentSubIndex = -1;
            }

            object IEnumerator.Current
            {
                get
                {
                    var entry = _parent.GetEntryByIndex(_currentIndex);
                    return new KeyValuePair<TKey, TValue>(entry.Key, entry[_currentSubIndex]);
                }
            }

            public KeyValuePair<TKey, TValue> Current
            {
                get
                {
                    var entry = _parent.GetEntryByIndex(_currentIndex);
                    return new KeyValuePair<TKey, TValue>(entry.Key, entry[_currentSubIndex]);
                }
            }
        }
    }
}