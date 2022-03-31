using System.Collections;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Toolkit.Diagnostics;
using static Alt.Hashing;

// see 2 previous implementations in comments below
namespace Alt
{
    public class AltDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    {
        public AltDictionary(int capacity, IComparer<TKey>? comparer)
        {
            if (capacity < 0)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException("Capacity must be a non-negative number.");
            }
            collectionCount = 0;
            collection = new Chain<TKey, TValue>[GetBucketCount(capacity)];
            InitialiseCollection();
            this.comparer = comparer ?? Comparer<TKey>.Default;
            keys = new List<TKey>();
            values = new List<TValue>();
        }

        public AltDictionary() : this(0, null) { }

        public AltDictionary(int capacity) : this(capacity, null) { }

        public AltDictionary(IComparer<TKey>? comparer) : this(0, comparer) { }

        public AltDictionary(AltDictionary<TKey, TValue> altDictionary, IComparer<TKey>? comparer)
        {
            collection = new Chain<TKey, TValue>[GetBucketCount(altDictionary.Count)];
            InitialiseCollection();
            foreach (KeyValuePair<TKey, TValue> item in altDictionary)
            {
                Add(item);
            }
            this.comparer = comparer ?? Comparer<TKey>.Default;
            keys = new List<TKey>();
            values = new List<TValue>();
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
                if (key != null)
                {
                    if (!collection[Hash(key, collection.Length)].Set(key, value, comparer))
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

        public ICollection<TKey> Keys
        {
            get
            {
                keys = new List<TKey>();
                for (int i = 0; i < collection.Length; i++)
                {
                    foreach (var item in collection[i])
                    {
                        keys.Add(item.Key);
                    }
                }
                return keys;
            }
        }

        public ICollection<TValue> Values
        {
            get
            {
                values = new List<TValue>();
                for (int i = 0; i < collection.Length; i++)
                {
                    foreach (var item in collection[i])
                    {
                        values.Add(item.Value);
                    }
                }
                return values;
            }
        }

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
            collection[Hash(key, collection.Length)].Add(new KeyValuePair<TKey, TValue>(key, value));
            UpdateCollectionAfterAdding();
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            if (!ContainsKey(item.Key))
            {
                collection[Hash(item.Key, collection.Length)].Add(item);
                UpdateCollectionAfterAdding();
            }
        }

        public void Clear()
        {
            collectionCount = 0;
            collection = new Chain<TKey, TValue>[GetBucketCount(collection.Length)];
            InitialiseCollection();
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return collection[Hash(item.Key, collection.Length)].Contains(item);
        }

        public bool ContainsKey(TKey key)
        {
            if (key == null)
            {
                ThrowHelper.ThrowArgumentNullException("Key value cannot be null.");
            }
            return collectionCount > 0 && collection[Hash(key, collection.Length)].Any(x => comparer.Compare(x.Key, key) == 0);
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
            for (int i = 0, j = arrayIndex; i < collection.Length; i++)
            {
                foreach (KeyValuePair<TKey, TValue> item in collection[i])
                {
                    array[j++] = item;
                }
            }
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            for (int i = 0; i < collection.Length; i++)
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
            return collection[Hash(key, collection.Length)].Remove(key, comparer);
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
            return collection[Hash(key, collection.Length)].Get(key, out value, comparer);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private void UpdateCollectionAfterAdding()
        {
            if (++collectionCount >= collection.Length)
            //if (++collectionCount >= 0.7 * collection.Length)
            {
                int newBucketCount = GetBucketCount(collectionCount);
                if (newBucketCount > collection.Length)
                {
                    Rehash(newBucketCount);
                }
            }
        }

        private void Rehash(int newBucketCount)
        {
            var newCollection = new Chain<TKey, TValue>[newBucketCount];
            for (int i = 0; i < newCollection.Length; i++)
            {
                newCollection[i] = new Chain<TKey, TValue>();
            }
            for (int i = 0; i < collection.Length; i++)
            {
                foreach (KeyValuePair<TKey, TValue> item in collection[i])
                {
                    newCollection[Hash(item.Key, newBucketCount)].Add(item);
                }
            }
            collection = newCollection;
        }

        private void InitialiseCollection()
        {
            for (int i = 0; i < collection.Length; i++)
            {
                collection[i] = new Chain<TKey,TValue>();
            }
        }

        private Chain<TKey, TValue>[] collection;
        private readonly IComparer<TKey> comparer;
        private int collectionCount;
        private ICollection<TKey> keys;
        private ICollection<TValue> values;
    }

    //public class AltDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    //{
    //    public AltDictionary(int capacity, IComparer<TKey>? comparer)
    //    {
    //        if (capacity < 0)
    //        {
    //            ThrowHelper.ThrowArgumentOutOfRangeException("Capacity must be a non-negative number.");
    //        }
    //        collectionCount = 0;
    //        collection = new List<KeyValuePair<TKey, TValue>>[GetBucketCount(capacity)];
    //        InitialiseCollection();
    //        this.comparer = comparer ?? Comparer<TKey>.Default;
    //        keys = new List<TKey>();
    //        values = new List<TValue>();
    //    }

    //    public AltDictionary() : this(0, null) { }

    //    public AltDictionary(int capacity) : this(capacity, null) { }

    //    public AltDictionary(IComparer<TKey>? comparer) : this(0, comparer) { }

    //    public AltDictionary(AltDictionary<TKey, TValue> altDictionary, IComparer<TKey>? comparer)
    //    {
    //        collection = new List<KeyValuePair<TKey, TValue>>[GetBucketCount(altDictionary.Count)];
    //        InitialiseCollection();
    //        foreach (KeyValuePair<TKey, TValue> item in altDictionary)
    //        {
    //            Add(item);
    //        }
    //        this.comparer = comparer ?? Comparer<TKey>.Default;
    //        keys = new List<TKey>();
    //        values = new List<TValue>();
    //    }

    //    public AltDictionary(AltDictionary<TKey, TValue> altDictionary) : this(altDictionary, null) { }

    //    public TValue this[TKey key]
    //    {
    //        get
    //        {
    //            if (!TryGetValue(key, out TValue? value))
    //            {
    //                // KeyNotFoundException not supported in ThrowHelper? :(
    //                throw new KeyNotFoundException("There is no key-value pair with such a key.");
    //            }
    //            return value;
    //        }
    //        set
    //        {
    //            if (key != null)
    //            {
    //                int index = collection[Hash(key, collection.Length)].FindIndex(x => comparer.Compare(x.Key, key) == 0);
    //                if (index >= 0)
    //                {
    //                    collection[Hash(key, collection.Length)][index] = new KeyValuePair<TKey, TValue>(key, value);
    //                }
    //                else
    //                {
    //                    throw new KeyNotFoundException("There is no key-value pair with such a key.");
    //                }
    //            }
    //            else
    //            {
    //                ThrowHelper.ThrowArgumentNullException("Key value cannot be null.");
    //            }
    //        }
    //    }

    //    public ICollection<TKey> Keys
    //    {
    //        get
    //        {
    //            keys = new List<TKey>();
    //            for (int i = 0; i < collection.Length; i++)
    //            {
    //                foreach (var item in collection[i])
    //                {
    //                    keys.Add(item.Key);
    //                }
    //            }
    //            return keys;
    //        }
    //    }

    //    public ICollection<TValue> Values
    //    {
    //        get
    //        {
    //            values = new List<TValue>();
    //            for (int i = 0; i < collection.Length; i++)
    //            {
    //                foreach (var item in collection[i])
    //                {
    //                    values.Add(item.Value);
    //                }
    //            }
    //            return values;
    //        }
    //    }

    //    public int Count => collectionCount;

    //    public bool IsReadOnly => false;

    //    public void Add(TKey key, TValue value)
    //    {
    //        if (key == null)
    //        {
    //            ThrowHelper.ThrowArgumentNullException("Key value cannot be null.");
    //        }
    //        if (ContainsKey(key))
    //        {
    //            ThrowHelper.ThrowArgumentException("A value with that key already exists.");
    //        }
    //        collection[Hash(key, collection.Length)].Add(new KeyValuePair<TKey, TValue>(key, value));
    //        UpdateCollectionAfterAdding();
    //    }

    //    public void Add(KeyValuePair<TKey, TValue> item)
    //    {
    //        if (!ContainsKey(item.Key))
    //        {
    //            collection[Hash(item.Key, collection.Length)].Add(item);
    //            UpdateCollectionAfterAdding();
    //        }
    //    }

    //    public void Clear()
    //    {
    //        collectionCount = 0;
    //        collection = new List<KeyValuePair<TKey, TValue>>[GetBucketCount(collection.Length)];
    //        InitialiseCollection();
    //    }

    //    public bool Contains(KeyValuePair<TKey, TValue> item)
    //    {
    //        return collection[Hash(item.Key, collection.Length)].Contains(item);
    //    }

    //    public bool ContainsKey(TKey key)
    //    {
    //        if (key == null)
    //        {
    //            ThrowHelper.ThrowArgumentNullException("Key value cannot be null.");
    //        }
    //        return collectionCount > 0 && collection[Hash(key, collection.Length)].Any(x => comparer.Compare(x.Key, key) == 0);
    //    }

    //    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    //    {
    //        if (array == null)
    //        {
    //            ThrowHelper.ThrowArgumentNullException("Array value cannot be null.");
    //        }
    //        if (arrayIndex < 0)
    //        {
    //            ThrowHelper.ThrowArgumentOutOfRangeException("Array index must be a non-negative integer.");
    //        }
    //        if (array.Length - arrayIndex < collectionCount)
    //        {
    //            throw new ArgumentException("Array cannot hold all elements.");
    //        }
    //        for (int i = 0, j = arrayIndex; i < collection.Length; i++)
    //        {
    //            foreach (KeyValuePair<TKey, TValue> item in collection[i])
    //            {
    //                array[j++] = item;
    //            }
    //        }
    //    }

    //    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    //    {
    //        for (int i = 0; i < collection.Length; i++)
    //        {
    //            foreach (KeyValuePair<TKey, TValue> item in collection[i])
    //            {
    //                yield return item;
    //            }
    //        }
    //    }

    //    public bool Remove(TKey key)
    //    {
    //        if (key == null)
    //        {
    //            ThrowHelper.ThrowArgumentNullException("Key value cannot be null.");
    //        }
    //        int index = collection[Hash(key, collection.Length)].FindIndex(x => comparer.Compare(x.Key, key) == 0);
    //        if (index >= 0)
    //        {
    //            collection[Hash(key, collection.Length)].RemoveAt(index);
    //            return true;
    //        }
    //        return false;
    //    }

    //    public bool Remove(KeyValuePair<TKey, TValue> item)
    //    {
    //        if (item.Key != null)
    //        {
    //            return Remove(item.Key);
    //        }
    //        return false;
    //    }

    //    public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value)
    //    {
    //        if (key == null)
    //        {
    //            ThrowHelper.ThrowArgumentNullException("Key value cannot be null.");
    //        }
    //        int index = collection[Hash(key, collection.Length)].FindIndex(x => comparer.Compare(x.Key, key) == 0);
    //        value = index == -1 ? default : collection[Hash(key, collection.Length)][index].Value;
    //        return index != -1;
    //    }

    //    IEnumerator IEnumerable.GetEnumerator()
    //    {
    //        return GetEnumerator();
    //    }

    //    private void UpdateCollectionAfterAdding()
    //    {
    //        if (++collectionCount >= collection.Length)
    //        //if (++collectionCount >= 0.7 * collection.Length)
    //        {
    //            int newBucketCount = GetBucketCount(collectionCount);
    //            if (newBucketCount > collection.Length)
    //            {
    //                Rehash(newBucketCount);
    //            }
    //        }
    //    }

    //    private void Rehash(int newBucketCount)
    //    {
    //        var newCollection = new List<KeyValuePair<TKey, TValue>>[newBucketCount];
    //        for (int i = 0; i < newCollection.Length; i++)
    //        {
    //            newCollection[i] = new List<KeyValuePair<TKey, TValue>>(1);
    //        }
    //        for (int i = 0; i < collection.Length; i++)
    //        {
    //            foreach (KeyValuePair<TKey, TValue> item in collection[i])
    //            {
    //                newCollection[Hash(item.Key, newBucketCount)].Add(item);
    //            }
    //        }
    //        collection = newCollection;
    //    }

    //    private void InitialiseCollection()
    //    {
    //        for (int i = 0; i < collection.Length; i++)
    //        {
    //            collection[i] = new List<KeyValuePair<TKey, TValue>>(1);
    //        }
    //    }

    //    private List<KeyValuePair<TKey, TValue>>[] collection;
    //    private readonly IComparer<TKey> comparer;
    //    private int collectionCount;
    //    private ICollection<TKey> keys;
    //    private ICollection<TValue> values;
    //}

    //public class AltDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    //{
    //    public AltDictionary(int capacity, IComparer<TKey>? comparer)
    //    {
    //        if (capacity < 0)
    //        {
    //            ThrowHelper.ThrowArgumentOutOfRangeException("Capacity must be a non-negative number.");
    //        }
    //        collectionCount = 0;
    //        collection = new RedBlackTree<TKey, TValue>[GetBucketCount(capacity)];
    //        InitialiseCollection();
    //        this.comparer = comparer ?? Comparer<TKey>.Default;
    //        keys = new List<TKey>();
    //        values = new List<TValue>();
    //    }

    //    public AltDictionary() : this(0, null) { }

    //    public AltDictionary(int capacity) : this(capacity, null) { }

    //    public AltDictionary(IComparer<TKey>? comparer) : this(0, comparer) { }

    //    public AltDictionary(RedBlackTree<TKey, TValue> altDictionary, IComparer<TKey>? comparer)
    //    {
    //        collection = new RedBlackTree<TKey, TValue>[GetBucketCount(altDictionary.Count)];
    //        InitialiseCollection();
    //        foreach (Node<TKey, TValue> item in altDictionary)
    //        {
    //            Add(item.Key, item.Value);
    //        }
    //        this.comparer = comparer ?? Comparer<TKey>.Default;
    //        keys = new List<TKey>();
    //        values = new List<TValue>();
    //    }

    //    public AltDictionary(RedBlackTree<TKey, TValue> altDictionary) : this(altDictionary, null) { }

    //    public TValue this[TKey key]
    //    {
    //        get
    //        {
    //            if (!TryGetValue(key, out TValue? value))
    //            {
    //                // KeyNotFoundException not supported in ThrowHelper? :(
    //                throw new KeyNotFoundException("There is no key-value pair with such a key.");
    //            }
    //            return value;
    //        }
    //        set
    //        {
    //            if (key != null)
    //            {
    //                var node = collection[Hash(key, collection.Length)].GetNode(key);
    //                if (node != null)
    //                {
    //                    node.Value = value;
    //                }
    //                else
    //                {
    //                    throw new KeyNotFoundException("There is no key-value pair with such a key.");
    //                }
    //            }
    //            else
    //            {
    //                ThrowHelper.ThrowArgumentNullException("Key value cannot be null.");
    //            }
    //        }
    //    }

    //    public ICollection<TKey> Keys
    //    {
    //        get
    //        {
    //            keys = new List<TKey>();
    //            for (int i = 0; i < collection.Length; i++)
    //            {
    //                foreach (var item in collection[i])
    //                {
    //                    keys.Add(item.Key);
    //                }
    //            }
    //            return keys;
    //        }
    //    }

    //    public ICollection<TValue> Values
    //    {
    //        get
    //        {
    //            values = new List<TValue>();
    //            for (int i = 0; i < collection.Length; i++)
    //            {
    //                foreach (var item in collection[i])
    //                {
    //                    values.Add(item.Value);
    //                }
    //            }
    //            return values;
    //        }
    //    }

    //    public int Count => collectionCount;

    //    public bool IsReadOnly => false;

    //    public void Add(TKey key, TValue value)
    //    {
    //        if (key == null)
    //        {
    //            ThrowHelper.ThrowArgumentNullException("Key value cannot be null.");
    //        }
    //        if (ContainsKey(key))
    //        {
    //            ThrowHelper.ThrowArgumentException("A value with that key already exists.");
    //        }
    //        collection[Hash(key, collection.Length)].Add(key, value);
    //        UpdateCollectionAfterAdding();
    //    }

    //    public void Add(KeyValuePair<TKey, TValue> item)
    //    {
    //        if (!ContainsKey(item.Key))
    //        {
    //            collection[Hash(item.Key, collection.Length)].Add(item.Key, item.Value);
    //            UpdateCollectionAfterAdding();
    //        }
    //    }

    //    public void Clear()
    //    {
    //        collectionCount = 0;
    //        collection = new RedBlackTree<TKey, TValue>[GetBucketCount(collection.Length)];
    //        InitialiseCollection();
    //    }

    //    public bool Contains(KeyValuePair<TKey, TValue> item)
    //    {
    //        return collection[Hash(item.Key, collection.Length)].Contains(item.Key, item.Value);
    //    }

    //    public bool ContainsKey(TKey key)
    //    {
    //        if (key == null)
    //        {
    //            ThrowHelper.ThrowArgumentNullException("Key value cannot be null.");
    //        }
    //        return collectionCount > 0 && collection[Hash(key, collection.Length)].Any(x => comparer.Compare(x.Key, key) == 0);
    //    }

    //    public bool Remove(TKey key)
    //    {
    //        if (key == null)
    //        {
    //            ThrowHelper.ThrowArgumentNullException("Key value cannot be null.");
    //        }
    //        return collection[Hash(key, collection.Length)].Remove(key);
    //    }

    //    public bool Remove(KeyValuePair<TKey, TValue> item)
    //    {
    //        if (item.Key != null)
    //        {
    //            return Remove(item.Key);
    //        }
    //        return false;
    //    }

    //    public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value)
    //    {
    //        if (key == null)
    //        {
    //            ThrowHelper.ThrowArgumentNullException("Key value cannot be null.");
    //        }
    //        var node = collection[Hash(key, collection.Length)].GetNode(key);
    //        value = (node == null) ? default(TValue) : node.Value;
    //        return node != null;
    //    }

    //    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    //    {
    //        for (int i = 0; i < collection.Length; i++)
    //        {
    //            foreach (Node<TKey, TValue> item in collection[i])
    //            {
    //                yield return new KeyValuePair<TKey, TValue>(item.Key, item.Value);
    //            }
    //        }
    //    }

    //    IEnumerator IEnumerable.GetEnumerator()
    //    {
    //        return GetEnumerator();
    //    }

    //    private void UpdateCollectionAfterAdding()
    //    {
    //        if (++collectionCount == collection.Length)
    //        //if (++collectionCount >= 0.7 * collection.Length)
    //        {
    //            int newBucketCount = GetBucketCount(collectionCount);
    //            if (newBucketCount > collection.Length)
    //            {
    //                Rehash(newBucketCount);
    //            }
    //        }
    //    }

    //    private void Rehash(int newBucketCount)
    //    {
    //        var newCollection = new RedBlackTree<TKey, TValue>[newBucketCount];
    //        for (int i = 0; i < newCollection.Length; i++)
    //        {
    //            newCollection[i] = new RedBlackTree<TKey, TValue>();
    //        }
    //        for (int i = 0; i < collection.Length; i++)
    //        {
    //            foreach (Node<TKey, TValue> item in collection[i])
    //            {
    //                newCollection[Hash(item.Key, newBucketCount)].Add(item);
    //            }
    //        }
    //        collection = newCollection;
    //    }

    //    private void InitialiseCollection()
    //    {
    //        for (int i = 0; i < collection.Length; i++)
    //        {
    //            collection[i] = new RedBlackTree<TKey, TValue>();
    //        }
    //    }

    //    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    //    {
    //        if (array == null)
    //        {
    //            ThrowHelper.ThrowArgumentNullException("Array value cannot be null.");
    //        }
    //        if (arrayIndex < 0)
    //        {
    //            ThrowHelper.ThrowArgumentOutOfRangeException("Array index must be a non-negative integer.");
    //        }
    //        if (array.Length - arrayIndex < collectionCount)
    //        {
    //            throw new ArgumentException("Array cannot hold all elements.");
    //        }
    //        for (int i = 0, j = arrayIndex; i < collection.Length; i++)
    //        {
    //            foreach (var item in collection[i])
    //            {
    //                array[j++] = new KeyValuePair<TKey, TValue>(item.Key, item.Value);
    //            }
    //        }
    //    }

    //    private RedBlackTree<TKey, TValue>[] collection;
    //    private readonly IComparer<TKey> comparer;
    //    private int collectionCount;
    //    private ICollection<TKey> keys;
    //    private ICollection<TValue> values;
    //}
}