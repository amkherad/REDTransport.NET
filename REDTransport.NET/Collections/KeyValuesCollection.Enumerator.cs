using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace REDTransport.NET.Collections
{
    public partial class KeyValuesCollection<TKey, TValue>
    {
        protected class Enumerator : IEnumerator<KeyValuePair<TKey, TValue>>, IDictionaryEnumerator
        {
            private readonly KeyValuesCollection<TKey, TValue> _parent;

            private int _entriesCount;
            private int _currentIndex;
            private int _currentSubIndex;

            private Entry _currentEntry;

            private TValue _current;
            //private TKey _currentKey;


            public Enumerator(KeyValuesCollection<TKey, TValue> collection)
            {
                _parent = collection;

                _entriesCount = _parent._entries.Count;
                _currentIndex = 0;
                _currentSubIndex = -1;
                _currentEntry = null;
                _current = default;
            }

            public void Dispose()
            {
                //_parent = null;
                _entriesCount = _parent._entries.Count;
                _currentIndex = 0;
                _currentSubIndex = -1;
                _currentEntry = null;
                _current = default;
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
                    
                    col = _parent.GetEntryByIndex(_currentIndex);
                    count = col.Count;
                }

                _currentEntry = col;
                _current = col[_currentSubIndex];

                return true;
            }

            public void Reset()
            {
                _entriesCount = _parent._entries.Count;
                _currentIndex = 0;
                _currentSubIndex = -1;
                _currentEntry = null;
                _current = default;
            }

            object IEnumerator.Current =>
                new KeyValuePair<TKey, TValue>(_currentEntry.Key, _current);

            public KeyValuePair<TKey, TValue> Current =>
                new KeyValuePair<TKey, TValue>(_currentEntry.Key, _current);

            public DictionaryEntry Entry => new DictionaryEntry(_currentEntry.Key, _current);

            public object Key => _currentEntry.Key;

            public object Value
            {
                get
                {
                    var val = _current;

                    return val as string;
                }
            }
        }
    }
}