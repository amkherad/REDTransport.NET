using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace REDTransport.NET.Collections
{
    /// <summary>
    /// Represents a collection of key/values, it supports duplicity in keys and values.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    [DebuggerDisplay("KeyCount={KeyCount}, EntryCount={Count}")]
    public partial class KeyValuesCollection<TKey, TValue> :
        ICollection<KeyValuePair<TKey, TValue>>,
        IEnumerable<KeyValuePair<TKey, TValue>>,
        IDictionary<TKey, TValue>,
        IDictionary<TKey, IEnumerable<TValue>>
    {
        private readonly Dictionary<TKey, Entry> _entries;
        private readonly List<TKey> _orderedKeys;
        private readonly IEqualityComparer<TKey> _keyComparer;
        private readonly IEqualityComparer<TValue> _valueComparer;
        private KeyCollection _keyCollection;
        private EnumerableValueCollection _valuesCollection;

        private bool _removeEmptyKeys = true;
        //private ICollection<TKey> _keys;
        //private ICollection<TValue> _values;
        //private ICollection<TKey> _keys1;
        //private ICollection<IEnumerable<TValue>> _values1;

        public KeyValuesCollection()
        {
            _keyComparer = EqualityComparer<TKey>.Default;
            _valueComparer = EqualityComparer<TValue>.Default;
            _entries = new Dictionary<TKey, Entry>(_keyComparer);
            _orderedKeys = new List<TKey>();
        }

        public KeyValuesCollection(IEqualityComparer<TKey> comparer)
        {
            _keyComparer = comparer;
            _valueComparer = EqualityComparer<TValue>.Default;
            _entries = new Dictionary<TKey, Entry>(comparer);
            _orderedKeys = new List<TKey>();
        }

        public KeyValuesCollection(int capacity)
        {
            _keyComparer = EqualityComparer<TKey>.Default;
            _valueComparer = EqualityComparer<TValue>.Default;
            _entries = new Dictionary<TKey, Entry>(capacity, _keyComparer);
            _orderedKeys = new List<TKey>(capacity);
        }

        public KeyValuesCollection(int capacity, IEqualityComparer<TKey> comparer)
        {
            _keyComparer = EqualityComparer<TKey>.Default;
            _valueComparer = EqualityComparer<TValue>.Default;
            _entries = new Dictionary<TKey, Entry>(capacity, comparer);
            _orderedKeys = new List<TKey>(capacity);
        }

        public KeyValuesCollection(IEnumerable<KeyValuePair<TKey, TValue>> values)
        {
            _keyComparer = EqualityComparer<TKey>.Default;
            _valueComparer = EqualityComparer<TValue>.Default;

            var vals = values.ToList();
            var capacity = vals.Select(v => v.Key).Distinct(_keyComparer).Count();

            _entries = new Dictionary<TKey, Entry>(capacity, _keyComparer);
            _orderedKeys = new List<TKey>(capacity);

            foreach (var elem in vals)
            {
                Add(elem.Key, elem.Value);
            }
        }

        public KeyValuesCollection(IDictionary<TKey, TValue> values)
        {
            var capacity = values.Keys.Count;

            _keyComparer = EqualityComparer<TKey>.Default;
            _valueComparer = EqualityComparer<TValue>.Default;
            _entries = new Dictionary<TKey, Entry>(capacity, _keyComparer);
            _orderedKeys = new List<TKey>(capacity);

            foreach (var elem in values)
            {
                Add(elem.Key, elem.Value);
            }
        }


        /// <summary>
        /// Checks whether the key exists in the collection or not.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public virtual bool ContainsKey(TKey key) => _entries.ContainsKey(key);


        /// <summary>
        /// Checks whether the key and value pair exists in the collection or not.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public virtual bool ContainsKeyValue(TKey key, TValue value)
        {
            var entry = GetEntryByKey(key);

            if (entry == null) return false;

            return _valueComparer == null
                ? entry.Contains(value)
                : entry.Contains(value, _valueComparer);
        }

        /// <summary>
        /// Checks whether the key and value pair exists in the collection or not, using an equality comparer for values.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="valueComparer"></param>
        /// <returns></returns>
        public virtual bool ContainsKeyValue(TKey key, TValue value, IEqualityComparer<TValue> valueComparer)
        {
            var entry = GetEntryByKey(key);

            if (entry == null) return false;

            return entry.Contains(value, valueComparer ?? _valueComparer);
        }

        /// <summary>
        /// Checks whether the key and all values exists in the collection or not, using an equality comparer for values.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="values"></param>
        /// <param name="valueComparer"></param>
        /// <returns></returns>
        public virtual bool ContainsKeyValues(TKey key, IEnumerable<TValue> values,
            IEqualityComparer<TValue> valueComparer)
        {
            var entry = GetEntryByKey(key);

            if (entry == null) return false;

            return values.All(v => entry.Contains(v, valueComparer ?? _valueComparer));
        }


        /// <summary>
        /// Checks whether the value exists in the collection or not.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public virtual bool ContainsValue(TValue value)
        {
            if (_valueComparer == null)
            {
                foreach (var kv in _entries)
                    if (kv.Value.Contains(value))
                        return true;
            }
            else
            {
                foreach (var kv in _entries)
                    if (kv.Value.Contains(value, _valueComparer))
                        return true;
            }

            return false;
        }

        /// <summary>
        /// Checks whether the value exists in the collection or not using an equality comparer for values.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="valueComparer"></param>
        /// <returns></returns>
        public virtual bool ContainsValue(TValue value, IEqualityComparer<TValue> valueComparer)
        {
            foreach (var kv in _entries)
                if (kv.Value.Contains(value, valueComparer ?? _valueComparer))
                    return true;

            return false;
        }

        public bool Contains(KeyValuePair<TKey, IEnumerable<TValue>> item)
        {
            return ContainsKeyValues(item.Key, item.Value, null);
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return ContainsKeyValue(item.Key, item.Value, null);
        }

        public bool Contains(IEnumerable<TValue> items)
        {
            var iterations = 0;

            foreach (var item in items)
            {
                if (!_entries.Any(e => e.Value.Contains(item)))
                {
                    return false;
                }

                iterations++;
            }

            return iterations > 0;
        }


        /// <summary>
        /// Counts the number of values for a key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public virtual int CountEntries(TKey key) => GetEntryByKey(key)?.Count ?? 0;

        public virtual IEnumerable<TValue> this[TKey key]
        {
            get { return GetEntryByKeyOrThrow(key); }
            set
            {
                if (_entries.ContainsKey(key))
                {
                    _entries[key] = new Entry(key, value);
                }
                else
                {
                    _entries.Add(key, new Entry(key, value));
                    _orderedKeys.Add(key);
                }
            }
        }

        TValue IDictionary<TKey, TValue>.this[TKey key]
        {
            get => GetEntryByKeyOrThrow(key).First();
            set
            {
                if (_entries.ContainsKey(key))
                {
                    _entries[key].Add(value);
                }
                else
                {
                    _entries.Add(key, new Entry(key, new[] {value}));
                    _orderedKeys.Add(key);
                }
            }
        }

        public virtual IEnumerable<TValue> this[int index]
        {
            get { return GetEntryByIndex(index); }
            set
            {
                var key = _orderedKeys[index];
                _entries[key] = new Entry(key, value);
            }
        }


        public virtual IEnumerable<TValue> Get(TKey key) => GetEntryByKey(key);

        public virtual void Set(TKey key, IEnumerable<TValue> values)
        {
            if (_entries.ContainsKey(key))
            {
                _entries[key] = new Entry(key, values);
            }
            else
            {
                _entries.Add(key, new Entry(key, values));
                _orderedKeys.Add(key);
            }
        }

        public virtual void Set(TKey key, TValue value) => Set(key, new[] {value});


        /// <summary>
        /// Replaces all values of a key with new ones.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public virtual IEnumerable<TValue> ReplaceValuesOfKey(TKey key, IEnumerable<TValue> values)
        {
            if (_entries.TryGetValue(key, out var entries))
            {
                _entries[key] = new Entry(key, values);
                return entries;
            }
            else
            {
                _entries.Add(key, new Entry(key, values));
                _orderedKeys.Add(key);
                return Enumerable.Empty<TValue>();
            }
        }

        /// <summary>
        /// Tries to get values.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public virtual bool TryGetValues(TKey key, out IEnumerable<TValue> values)
        {
            var result = _entries.TryGetValue(key, out var entries);
            values = entries;
            return result;
        }

        /// <summary>
        /// Tries to get value.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        bool IDictionary<TKey, IEnumerable<TValue>>.TryGetValue(TKey key, out IEnumerable<TValue> values)
        {
            return TryGetValues(key, out values);
        }

        /// <summary>
        /// Tries to get value.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryGetValue(TKey key, out TValue value)
        {
            var entry = GetEntryByKey(key);

            if (entry == null || entry.Count == 0)
            {
                value = default;
                return false;
            }

            value = entry.First();
            return true;
        }

        /// <summary>
        /// Sets all values for a key.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="values"></param>
        public virtual void TryAddValues(TKey key, IEnumerable<TValue> values)
        {
            if (_entries.ContainsKey(key))
            {
                var entry = _entries[key];
                foreach (var value in values)
                {
                    if (!entry.Contains(value))
                    {
                        entry.Add(value);
                    }
                }
            }
            else
            {
                _entries.Add(key, new Entry(key, values));
                _orderedKeys.Add(key);
            }
        }

        /// <summary>
        /// Sets all values for a key.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="values"></param>
        /// <param name="valueComparer"></param>
        public virtual void TryAddValues(TKey key, IEnumerable<TValue> values, IEqualityComparer<TValue> valueComparer)
        {
            if (_entries.ContainsKey(key))
            {
                var entry = _entries[key];
                foreach (var value in values)
                {
                    if (!entry.Contains(value, valueComparer ?? _valueComparer))
                    {
                        entry.Add(value);
                    }
                }
            }
            else
            {
                _entries.Add(key, new Entry(key, values));
                _orderedKeys.Add(key);
            }
        }

        /// <summary>
        /// Clears the collection.
        /// </summary>
        public virtual void Clear()
        {
            _entries.Clear();
            _orderedKeys.Clear();
        }

        /// <summary>
        /// Gets the first value of a key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        /// <exception cref="KeyNotFoundException"></exception>
        public virtual TValue First(TKey key)
        {
            var entry = GetEntryByKey(key);
            if (entry == null) throw new KeyNotFoundException();
            return entry.First();
        }

        /// <summary>
        /// Gets the first or default value of a key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public virtual TValue FirstOrDefault(TKey key)
        {
            var entry = GetEntryByKey(key);
            if (entry == null) return default;
            return entry.FirstOrDefault();
        }

        /// <summary>
        /// Gets a single value of a key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        /// <exception cref="KeyNotFoundException"></exception>
        public virtual TValue Single(TKey key)
        {
            var entry = GetEntryByKey(key);
            if (entry == null) throw new KeyNotFoundException();
            return entry.Single();
        }

        /// <summary>
        /// Gets a single or default value of a key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public virtual TValue SingleOrDefault(TKey key)
        {
            var entry = GetEntryByKey(key);
            if (entry == null) return default;
            return entry.SingleOrDefault();
        }

        /// <summary>
        /// Gets the last value of a key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        /// <exception cref="KeyNotFoundException"></exception>
        public virtual TValue Last(TKey key)
        {
            var entry = GetEntryByKey(key);
            if (entry == null) throw new KeyNotFoundException();
            return entry.Last();
        }

        /// <summary>
        /// Gets the last or default value of a key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public virtual TValue LastOrDefault(TKey key)
        {
            var entry = GetEntryByKey(key);
            if (entry == null) return default;
            return entry.LastOrDefault();
        }

        /// <summary>
        /// Adds a key/value item to collection. 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public virtual void Add(TKey key, TValue value)
        {
            var col = GetOrNewEntryByKey(key);
            col.Add(value);
        }

        /// <summary>
        /// Adds a key/value pair to collection. 
        /// </summary>
        /// <param name="item"></param>
        public virtual void Add(KeyValuePair<TKey, TValue> item)
        {
            var col = GetOrNewEntryByKey(item.Key);
            col.Add(item.Value);
        }

        /// <summary>
        /// Adds values to collection with specified key. 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="values"></param>
        public virtual void Add(TKey key, IEnumerable<TValue> values)
        {
            var col = GetOrNewEntryByKey(key);
            col.AddRange(values);
        }

        /// <summary>
        /// Adds values to collection with specified key. 
        /// </summary>
        /// <param name="item"></param>
        public virtual void Add(KeyValuePair<TKey, IEnumerable<TValue>> item)
        {
            var col = GetOrNewEntryByKey(item.Key);
            col.AddRange(item.Value);
        }

        /// <summary>
        /// Adds a range of values for a single key to collections.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="range"></param>
        public virtual void AddRange(TKey key, IEnumerable<TValue> range)
        {
            var col = GetOrNewEntryByKey(key);
            col.AddRange(range);
        }

        /// <summary>
        /// Adds a range of key/values to collections.
        /// </summary>
        /// <param name="items"></param>
        public virtual void AddRange(IEnumerable<KeyValuePair<TKey, IEnumerable<TValue>>> items)
        {
            if (items == null) throw new ArgumentNullException(nameof(items));

            foreach (var kv in items)
            {
                var col = GetOrNewEntryByKey(kv.Key);
                col.AddRange(kv.Value);
            }
        }

        /// <summary>
        /// Removes a key value pair.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public virtual bool Remove(TKey key, TValue value)
        {
            var col = GetEntryByKey(key);
            if (col == null)
            {
                return false;
            }

            var result = col.Remove(value);

            if (_removeEmptyKeys && col.Count == 0)
            {
                RemoveKey(key);
            }

            return result;
        }

        public virtual bool Remove(TKey key)
        {
            if (!_entries.ContainsKey(key))
            {
                return false;
            }

            _entries.Remove(key);
            return _orderedKeys.Remove(key);
        }

        public virtual bool Remove(KeyValuePair<TKey, TValue> item)
        {
            return Remove(item.Key, item.Value);
        }

        public bool Remove(KeyValuePair<TKey, IEnumerable<TValue>> item)
        {
            return Remove(item.Key, item.Value) > 0;
        }

        public int Remove(TKey key, IEnumerable<TValue> items)
        {
            var col = GetEntryByKey(key);
            if (col == null)
            {
                return 0;
            }

            var result = col.RemoveAll(r => items.Any(i => _valueComparer.Equals(r, i)));

            if (_removeEmptyKeys && col.Count == 0)
            {
                RemoveKey(key);
            }

            return result;
        }


        /// <summary>
        /// Removes all values with given key from collection.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="values"></param>
        public virtual void RemoveAll(TKey key, IEnumerable<TValue> values)
        {
            var col = GetEntryByKey(key);
            if (col == null)
            {
                return;
            }

            col.RemoveAll(r => values.Contains(r, _valueComparer));

            if (_removeEmptyKeys && col.Count == 0)
            {
                RemoveKey(key);
            }
        }

        /// <summary>
        /// Removes a key and all of it's values.
        /// </summary>
        /// <param name="key"></param>
        public virtual void RemoveKey(TKey key)
        {
            _entries.Remove(key);
            _orderedKeys.RemoveAll(r => _keyComparer.Equals(r, key));
        }

        /// <summary>
        /// Removes all values using default comparer.
        /// </summary>
        /// <param name="values"></param>
        public virtual void RemoveAllValues(IEnumerable<TValue> values)
        {
            var valuesArray = values.ToArray();
            foreach (var kv in _entries)
            {
                kv.Value.RemoveAll(r => valuesArray.Contains(r, _valueComparer));
            }
        }

        /// <summary>
        /// Removes all values using default comparer.
        /// </summary>
        /// <param name="values"></param>
        /// <param name="valueComparer"></param>
        public virtual void RemoveAllValues(IEnumerable<TValue> values, IEqualityComparer<TValue> valueComparer)
        {
            var valuesArray = values.ToArray();
            foreach (var kv in _entries)
            {
                kv.Value.RemoveAll(r => valuesArray.Contains(r, valueComparer));
            }
        }


        /// <summary>
        /// Gets all keys.
        /// </summary>
        public virtual ICollection<TKey> Keys => _keyCollection ??= new KeyCollection(this);

        /// <summary>
        /// Gets all values.
        /// </summary>
        public virtual ICollection<IEnumerable<TValue>> Values =>
            _valuesCollection ??= new EnumerableValueCollection(this);

        //public virtual IEnumerable<TValue> Values => _entries.SelectMany(entry => entry.Value);

        ICollection<IEnumerable<TValue>> IDictionary<TKey, IEnumerable<TValue>>.Values =>
            _valuesCollection ??= new EnumerableValueCollection(this);

        ICollection<TKey> IDictionary<TKey, IEnumerable<TValue>>.Keys => _keyCollection ??= new KeyCollection(this);

        ICollection<TValue> IDictionary<TKey, TValue>.Values => new ValueCollection(this);

        ICollection<TKey> IDictionary<TKey, TValue>.Keys => _keyCollection ??= new KeyCollection(this);

        /// <summary>
        /// Returns all entries as enumerable of KeyValuePairs.
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<KeyValuePair<TKey, TValue>> ToKeyValuePairs()
        {
            var allKeys = _entries.Keys;
            foreach (var key in allKeys)
            {
                foreach (var value in _entries[key])
                {
                    yield return new KeyValuePair<TKey, TValue>(key, value);
                }
            }
        }


        /// <summary>
        /// Gets or create a new entry.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        protected virtual Entry GetOrNewEntryByKey(TKey key)
        {
            if (!_entries.TryGetValue(key, out var result))
            {
                result = new Entry(key);
                _entries.Add(key, result);
                _orderedKeys.Add(key);
            }

            return result;
        }

        /// <summary>
        /// Gets an entry.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        protected virtual Entry GetEntryByKey(TKey key)
        {
            return _entries.TryGetValue(key, out var result)
                ? result
                : null;
        }

        /// <summary>
        /// Gets an entry or throws if key is not exist.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        protected virtual Entry GetEntryByKeyOrThrow(TKey key)
        {
            return _entries.TryGetValue(key, out var result)
                ? result
                : throw new KeyNotFoundException();
        }

        /// <summary>
        /// Gets an entry by index.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        protected virtual Entry GetEntryByIndex(int index)
        {
            return _entries[_orderedKeys[index]];
        }

        /// <summary>
        /// Returns the index of an entry specified by it's key. 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        protected virtual int? GetEntryIndexByKey(TKey key)
        {
            for (var i = 0; i < _orderedKeys.Count; i++)
            {
                if (_keyComparer.Equals(_orderedKeys[i], key))
                {
                    return i;
                }
            }

            return null;
        }


        public virtual void CopyTo(Array array, int index) => ((ICollection) _entries).CopyTo(array, index);

        public virtual void CopyTo(KeyValuePair<TKey, IEnumerable<TValue>>[] array, int arrayIndex)
        {
            for (var i = arrayIndex; i < _entries.Count; i++)
            {
                var key = _orderedKeys[i];
                array[i] = new KeyValuePair<TKey, IEnumerable<TValue>>(key, _entries[key]);
            }
        }

        public virtual void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            for (var i = arrayIndex; i < _entries.Count; i++)
            {
                var key = _orderedKeys[i];
                array[i] = new KeyValuePair<TKey, TValue>(key, _entries[key].First());
            }
        }

        public virtual bool Any
        {
            get
            {
                if (_entries.Count == 0) return false;

                return _entries.Any(e => e.Value.Count > 0);
            }
        }

        public virtual int KeyCount => _entries.Count;

        public virtual int Count
        {
            get
            {
                if (_entries.Count == 0)
                {
                    return 0;
                }

                return _entries.Sum(e => e.Value.Count);
            }
        }

        public bool IsReadOnly { get; }

        public virtual bool IsSynchronized => ((ICollection) _entries).IsSynchronized;
        public virtual object SyncRoot => ((ICollection) _entries).SyncRoot;


        IEnumerator<KeyValuePair<TKey, IEnumerable<TValue>>>
            IEnumerable<KeyValuePair<TKey, IEnumerable<TValue>>>.GetEnumerator()
        {
            foreach (var kv in _entries)
            {
                yield return new KeyValuePair<TKey, IEnumerable<TValue>>(kv.Key, kv.Value);
            }
        }

        IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator() =>
            new Enumerator(this);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public virtual IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => new Enumerator(this);
    }
}