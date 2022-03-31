using Microsoft.Toolkit.Diagnostics;
using System.Collections;

namespace Alt
{
    public class Chain<TKey, TValue> : ICollection<KeyValuePair<TKey, TValue>>
    {
        public Chain()
        {
            first = null;
            rest = null;
        }

        public int Count
        {
            get
            {
                if (first == null)
                {
                    return 0;
                }
                else if (rest == null)
                {
                    return 1;
                }
                return 1 + rest.Count;
            }
        }

        public bool IsReadOnly => false;

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            if (first == null)
            {
                first = item;
            }
            else if (rest == null)
            {
                rest = new List<KeyValuePair<TKey, TValue>>(1) { item };
            }
            else
            {
                rest.Add(item);
            }
        }

        public bool Get(TKey key, out TValue? value, IComparer<TKey> comparer)
        {
            if (first != null)
            {
                if (comparer.Compare(((KeyValuePair<TKey, TValue>)first).Key, key) == 0)
                {
                    value = ((KeyValuePair<TKey, TValue>)first).Value;
                    return true;
                }
                else
                {
                    if (rest != null)
                    {
                        var index = rest.FindIndex(x => comparer.Compare(x.Key, key) == 0);
                        if (index >= 0)
                        {
                            value = rest[index].Value;
                            return true;
                        }
                    }
                }
            }
            value = default;
            return false;
        }

        public bool Set(TKey key, TValue value, IComparer<TKey> comparer)
        {
            if (first != null)
            {
                if (comparer.Compare(((KeyValuePair<TKey,TValue>)first).Key, key) == 0)
                {
                    first = new KeyValuePair<TKey, TValue>(key, value);
                    return true;
                }
                else
                {
                    if (rest != null)
                    {
                        var index = rest.FindIndex(x => comparer.Compare(x.Key, key) == 0);
                        if (index >= 0)
                        {
                            rest[index] = new KeyValuePair<TKey, TValue>(key, value);
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public bool Remove(TKey key, IComparer<TKey> comparer)
        {
            if (first != null)
            {
                if (comparer.Compare(((KeyValuePair<TKey, TValue>)first).Key, key) == 0)
                {
                    if (rest == null || rest.Count == 0)
                    {
                        first = null;
                        return true;
                    }
                    else
                    {
                        {
                            first = rest[^1];
                            rest.RemoveAt(rest.Count - 1);
                            return true;
                        }
                    }
                }
                else if (rest != null)
                {
                    var index = rest.FindIndex(x => comparer.Compare(x.Key, key) == 0);
                    if (index >= 0)
                    {
                        rest.RemoveAt(index);
                        return true;
                    }
                }
            }
            return false;
        }

        public void Clear()
        {
            first = null;
            rest = null;
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return first != null && ( first.Equals(item) || (rest != null && rest.Contains(item)) );
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            if (array == null)
            {
                ThrowHelper.ThrowArgumentNullException(nameof(array));
            }
            else if (arrayIndex < array.Length - Count)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException(nameof(array));
            }
            if (first != null)
            {
                array[arrayIndex++] = (KeyValuePair<TKey, TValue>)first;
                if (rest != null)
                {
                    foreach (var item in rest)
                    {
                        array[arrayIndex++] = item;
                    }
                }
            }
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            if (first != null)
            {
                yield return (KeyValuePair<TKey, TValue>)first;
                if (rest != null)
                {
                    foreach (var item in rest)
                    {
                        yield return item;
                    }
                }
            }
            yield break;
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            if (first != null)
            {
                if (first.Equals(item))
                {
                    if (rest == null || rest.Count == 0)
                    {
                        first = null;
                        return true;
                    }
                    else
                    {
                        first = rest[^1];
                        rest.RemoveAt(rest.Count - 1);
                        return true;
                    }
                }
                else if (rest != null)
                {
                    return rest.Remove(item);
                }
            }
            return false;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private KeyValuePair<TKey, TValue>? first;
        private List<KeyValuePair<TKey, TValue>>? rest;
    }
}