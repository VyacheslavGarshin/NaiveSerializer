using System;
using System.IO;

namespace Naive.Serializer
{
    public class RomStream : Stream
    {
        public override bool CanRead { get; } = true;

        public override bool CanSeek { get; } = true;

        public override bool CanWrite { get; } = true;

        public override long Length => _readOnlyMemory.Length;

        public override long Position { get => _position; set { _position = value; } }

        private ReadOnlyMemory<byte> _readOnlyMemory;

        private long _position;

        public RomStream(ReadOnlyMemory<byte> readOnlyMemory)
        {
            _readOnlyMemory = readOnlyMemory;
        }

        public override void Flush()
        {
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            _readOnlyMemory.Slice((int)_position, count).CopyTo(offset != 0 ? buffer.AsMemory(offset, count) : buffer);
            _position += count;
            return count;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            _position += offset;
            return _position;
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }
    }
}
