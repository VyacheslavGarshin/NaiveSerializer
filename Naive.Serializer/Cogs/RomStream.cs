using System;
using System.IO;

namespace Naive.Serializer.Cogs
{
    /// <summary>
    /// Stream over the ReadOnlyMemory structure.
    /// </summary>
    public class RomStream : Stream
    {
        /// <inheritdoc/>
        public override bool CanRead { get; } = true;

        /// <inheritdoc/>
        public override bool CanSeek { get; } = true;

        /// <inheritdoc/>
        public override bool CanWrite { get; } = true;

        /// <inheritdoc/>
        public override long Length => _readOnlyMemory.Length;

        /// <inheritdoc/>
        public override long Position { get => _position; set { _position = value; } }

        private ReadOnlyMemory<byte> _readOnlyMemory;

        private long _position;

        /// <summary>
        /// Create a new stream over the ReadOnlyMemory structure.
        /// </summary>
        /// <param name="readOnlyMemory"></param>
        public RomStream(ReadOnlyMemory<byte> readOnlyMemory)
        {
            _readOnlyMemory = readOnlyMemory;
        }

        /// <inheritdoc/>
        public override void Flush()
        {
        }

        /// <inheritdoc/>
        public override int Read(byte[] buffer, int offset, int count)
        {
            _readOnlyMemory.Slice((int)_position, count).CopyTo(offset != 0 ? buffer.AsMemory(offset, count) : buffer);
            _position += count;
            return count;
        }

        /// <inheritdoc/>
        public override long Seek(long offset, SeekOrigin origin)
        {
            _position += offset;
            return _position;
        }

        /// <inheritdoc/>
        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc/>
        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }
    }
}
