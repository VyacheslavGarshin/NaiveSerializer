using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Naive.Serializer.Cogs
{
    public class QuickTable<T> : IEnumerable<T>
    {
        private static readonly BytesComparer _comparer = new();

        private readonly KeyValuePair<ReadOnlyMemory<byte>, T>? [] _table = new KeyValuePair<ReadOnlyMemory<byte>, T>?[byte.MaxValue + 1];

        public void Add(ReadOnlyMemory<byte> key, T value)
        {
            var hashCode = GetHashCode(key);

            if (_table[hashCode] == null)
            {
                _table[hashCode] = new KeyValuePair<ReadOnlyMemory<byte>, T>(key, value);
            }
        }

        public T Get(ReadOnlyMemory<byte> key, bool optimistic = false)
        {
            var hashCode = GetHashCode(key);

            var result = _table[hashCode];

            return result != null && (optimistic || _comparer.Equals(result.Value.Key, key)) ? result.Value.Value : default;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _table.Where(x => x != null).Select(x => x.Value.Value).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private byte GetHashCode(ReadOnlyMemory<byte> key)
        {
            var span = key.Span;
            var first = span[0];
            var hashCode = first;
            
            if (key.Length > 1)
            {
                var last = span[^1];
                hashCode = (byte)(hashCode ^ ((uint)last << 2));
            }

            return hashCode;
        }
    }
}
