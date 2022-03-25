using System.Collections;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Toolkit.Diagnostics;
using static Alt.Hashing;

namespace Alt
{
    public class AltDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    {
        public AltDictionary(int capacity, IEqualityComparer<TKey>? comparer)
        {
            if (capacity < 0)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException("Capacity must be a non-negative number.");
            }
            bucketCount = GetBucketCount(capacity);
            collectionCount = 0;
            collection = GetInitialisedCollection(bucketCount);
            this.comparer = comparer ?? EqualityComparer<TKey>.Default;
        }

        public AltDictionary() : this(0, null) { }

        public AltDictionary(int capacity) : this(capacity, null) { }

        public AltDictionary(IEqualityComparer<TKey>? comparer) : this(0, comparer) { }

        public AltDictionary(AltDictionary<TKey, TValue> altDictionary, IEqualityComparer<TKey>? comparer)
        {
            bucketCount = altDictionary.Count + 1;
            collection = new List<KeyValuePair<TKey, TValue>>[bucketCount];
            for (int i = 0; i < bucketCount; i++)
            {
                collection[i] = new List<KeyValuePair<TKey, TValue>>();
            }
            foreach (KeyValuePair<TKey, TValue> item in altDictionary)
            {
                Add(item);
            }
            this.comparer = comparer ?? EqualityComparer<TKey>.Default;
        }

        public AltDictionary(AltDictionary<TKey, TValue> altDictionary) : this(altDictionary, null) { }

        public TValue this[TKey key]
        {
            get
            {
                if (!TryGetValue(key, out TValue? value))
                {
                    // KeyNotFoundException not supported in ThrowHelper? :(
                    throw new KeyNotFoundException("There is no key-value pair with such a key.");
                }
                return value;
            }
            set
            {
                if (value != null)
                {
                    int index = collection[Hash(key, bucketCount)].FindIndex(x => comparer.Equals(x.Key, key));
                    if (index >= 0)
                    {
                        collection[Hash(key, bucketCount)][index] = new KeyValuePair<TKey, TValue>(key, value);
                    }
                    else
                    {
                        throw new KeyNotFoundException("There is no key-value pair with such a key.");
                    }
                }
                else
                {
                    ThrowHelper.ThrowArgumentNullException("Key value cannot be null.");
                }
            }
        }

        public ICollection<TKey> Keys => GetKeys();

        public ICollection<TValue> Values => GetValues();

        public int Count => collectionCount;

        public bool IsReadOnly => false;

        public void Add(TKey key, TValue value)
        {
            if (key == null)
            {
                ThrowHelper.ThrowArgumentNullException("Key value cannot be null.");
            }
            if (ContainsKey(key))
            {
                ThrowHelper.ThrowArgumentException("A value with that key already exists.");
            }
            collection[Hash(key, bucketCount)].Add(new KeyValuePair<TKey, TValue>(key, value));
            UpdateCollectionAfterAdding();
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            if (!ContainsKey(item.Key))
            {
                collection[Hash(item.Key, bucketCount)].Add(item);
                UpdateCollectionAfterAdding();
            }
        }

        public void Clear()
        {
            bucketCount = 3;
            collectionCount = 0;
            collection = GetInitialisedCollection(bucketCount);
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return collection[Hash(item.Key, bucketCount)].Contains(item);
        }

        public bool ContainsKey(TKey key)
        {
            if (key == null)
            {
                ThrowHelper.ThrowArgumentNullException("Key value cannot be null.");
            }
            return collectionCount > 0 && collection[Hash(key, bucketCount)].Any(x => comparer.Equals(x.Key, key));
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            if (array == null)
            {
                ThrowHelper.ThrowArgumentNullException("Array value cannot be null.");
            }
            if (arrayIndex < 0)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException("Array index must be a non-negative integer.");
            }
            if (array.Length - arrayIndex < collectionCount)
            {
                throw new ArgumentException("Array cannot hold all elements.");
            }
            for (int i = 0, j = arrayIndex; i < bucketCount; i++)
            {
                foreach (KeyValuePair<TKey, TValue> item in collection[i])
                {
                    array[j++] = item;
                }
            }
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            for (int i = 0; i < bucketCount; i++)
            {
                foreach (KeyValuePair<TKey, TValue> item in collection[i])
                {
                    yield return item;
                }
            }
        }

        public bool Remove(TKey key)
        {
            if (key == null)
            {
                ThrowHelper.ThrowArgumentNullException("Key value cannot be null.");
            }
            int index = collection[Hash(key, bucketCount)].FindIndex(x => comparer.Equals(x.Key, key));
            if (index >= 0)
            {
                collection[Hash(key, bucketCount)].RemoveAt(index);
                return true;
            }
            return false;
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            if (item.Key != null)
            {
                return Remove(item.Key);
            }
            return false;
        }

        public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value)
        {
            if (key == null)
            {
                ThrowHelper.ThrowArgumentNullException("Key value cannot be null.");
            }
            int index = collection[Hash(key, bucketCount)].FindIndex(x => comparer.Equals(x.Key, key));
            value = index == -1 ? default : collection[Hash(key, bucketCount)][index].Value;
            return index != -1;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private ICollection<TValue> GetValues()
        {
            var values = new List<TValue>();
            for (int i = 0; i < bucketCount; i++)
            {
                foreach (var item in collection[i])
                {
                    values.Add(item.Value);
                }
            }
            return values;
        }

        private ICollection<TKey> GetKeys()
        {
            var keys = new List<TKey>();
            for (int i = 0; i < bucketCount; i++)
            {
                foreach (var item in collection[i])
                {
                    keys.Add(item.Key);
                }
            }
            return keys;
        }

        private void UpdateCollectionAfterAdding()
        {
            if (++collectionCount == bucketCount)
            {
                int newBucketCount = GetBucketCount(collectionCount);
                if (newBucketCount > bucketCount)
                {
                    Rehash(newBucketCount);
                }
            }
        }

        private void Rehash(int newBucketCount)
        {
            List<KeyValuePair<TKey, TValue>>[] newCollection = GetInitialisedCollection(newBucketCount);      
            for (int i = 0; i < bucketCount; i++)
            {
                foreach (KeyValuePair<TKey, TValue> item in collection[i])
                {
                    newCollection[Hash(item.Key, newBucketCount)].Add(item);
                }
            }
            collection = newCollection;
            bucketCount = newBucketCount;
        }

        private static List<KeyValuePair<TKey, TValue>>[] GetInitialisedCollection(int bucketCount)
        {
            var collection = new List<KeyValuePair<TKey, TValue>>[bucketCount];
            for (int i = 0; i < bucketCount; i++)
            {
                collection[i] = new List<KeyValuePair<TKey, TValue>>();
            }
            return collection;
        }

        private List<KeyValuePair<TKey, TValue>>[] collection;
        private readonly IEqualityComparer<TKey> comparer;
        private int bucketCount;
        private int collectionCount;
    }
}