using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Alt
{
    public class AltDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    {
        public AltDictionary()
        {
            collection = new List<KeyValuePair<TKey, TValue>>();
            keys = new List<TKey>();
            values = new List<TValue>();
        }

        public TValue this[TKey key]
        {
            get
            {
                TValue? value;
                TryGetValue(key, out value);
                return value;
            }
            set
            {
                if (value != null)
                {
                    if (Remove(key))
                    {
                        collection.Add(new KeyValuePair<TKey, TValue>(key, value));
                    }
                    else
                    {
                        throw new KeyNotFoundException("There is no key-value pair with such a key.");
                    }

                }
                else
                {
                    throw new ArgumentNullException("Key value cannot be null.");
                }
            }
        }

        public ICollection<TKey> Keys => keys;

        public ICollection<TValue> Values => values;

        public int Count => collection.Count;

        public bool IsReadOnly => false;

        public void Add(TKey key, TValue value)
        {
            if (key == null)
            {
                throw new ArgumentNullException("Key value cannot be null.");
            }
            if (ContainsKey(key))
            {
                throw new ArgumentException("A value with that key already exists.");
            }
            collection.Add(new KeyValuePair<TKey, TValue>(key, value));
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            collection.Add(item);
        }

        public void Clear()
        {
            collection.Clear();
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return collection.Contains(item);
        }

        public bool ContainsKey(TKey key)
        {
            if (key == null)
            {
                throw new ArgumentNullException("Key value cannot be null.");
            }
            return collection.Any(x => EqualityComparer<TKey>.Default.Equals(x.Key, key));
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            if (array == null)
            {
                throw new ArgumentNullException("Array value cannot be null.");
            }
            if (arrayIndex < 0)
            {
                throw new ArgumentOutOfRangeException("Array index must be a non-negative integer.");
            }
            if (array.Length - arrayIndex < collection.Count)
            {
                throw new ArgumentException("Array cannot hold all elements.");
            }
            int i = arrayIndex;
            foreach (KeyValuePair<TKey, TValue> item in collection)
            {
                array[i++] = item;
            }
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return collection.GetEnumerator();
        }

        public bool Remove(TKey key)
        {
            if (key == null)
            {
                throw new ArgumentNullException("Key value cannot be null.");
            }
            return collection.RemoveAll(x => EqualityComparer<TKey>.Default.Equals(x.Key, key)) > 0;
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            return collection.Remove(item);
        }

        public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value)
        {
            if (key == null)
            {
                throw new ArgumentNullException("Key value cannot be null.");
            }
            KeyValuePair<TKey, TValue> element = collection.Find(x => EqualityComparer<TKey>.Default.Equals(x.Key, key));
            value = element.Value;
            return EqualityComparer<KeyValuePair<TKey, TValue>>.Default.Equals(element, default) ? false : true;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return collection.GetEnumerator();
        }

        private List<KeyValuePair<TKey, TValue>> collection;
        private List<TKey> keys;
        private List<TValue> values;
    }
}
