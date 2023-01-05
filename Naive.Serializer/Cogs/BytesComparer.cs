using System;
using System.Collections.Generic;

namespace Naive.Serializer.Cogs
{
    /// <summary>
    /// Comparer by array content.
    /// </summary>
    public class BytesComparer : IEqualityComparer<ReadOnlyMemory<byte>>
    {
        /// <inheritdoc/>
        public bool Equals(ReadOnlyMemory<byte> x, ReadOnlyMemory<byte> y)
        {
            if (x.Length != y.Length)
            {
                return false;
            };

            var spanX = x.Span;
            var spanY = y.Span;

            for (var i = 0; i < spanX.Length; i++)
            {
                if (spanX[i] != spanY[i])
                {
                    return false;
                }
            }

            return true;
        }

        /// <inheritdoc/>
        public int GetHashCode(ReadOnlyMemory<byte> obj)
        {
            var result = 0;
            var span = obj.Span;

            for (var i = 0; i < span.Length; i++)
            {
                result = HashCode.Combine(result, span[i]);
            }

            return result;
        }
    }
}
