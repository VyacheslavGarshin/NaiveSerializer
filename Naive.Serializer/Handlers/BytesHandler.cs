using Naive.Serializer.Cogs;
using System;

namespace Naive.Serializer.Handlers
{
    internal class BytesHandler : AbstractHandler
    {
        public override HandlerType HandlerType { get; } = HandlerType.Bytes;

        private readonly bool _isReadOnlyMemory;

        public BytesHandler(Type type) : base(type)
        {
            IsNullable = true; 
            IsSimple = false;

            if (type != null)
            {
                if (type == typeof(ReadOnlyMemory<byte>) || type == typeof(ReadOnlyMemory<byte>?))
                {
                    _isReadOnlyMemory = true;
                }

                if (type.IsValueType && Nullable.GetUnderlyingType(type) == null)
                {
                    IsNullable = false;
                }
            }
        }

        public override bool Match(Type type)
        {
            return type == typeof(byte[]) || type == typeof(ReadOnlyMemory<byte>) || type == typeof(ReadOnlyMemory<byte>?);
        }

        public override void Write(BinaryWriterInternal writer, object obj, Context context)
        {
            int length;
            ReadOnlySpan<byte> span;

            if (_isReadOnlyMemory)
            {
                var rom = (ReadOnlyMemory<byte>)obj;
                length = rom.Length;
                span = rom.Span;
            }
            else
            {
                var bytes = (byte[])obj;
                length = bytes.Length;
                span = bytes.AsSpan();
            }

            writer.Write7BitEncodedInt(length);

            if (length > 0)
            {
                writer.Write(span);
            }
        }

        public override object Read(BinaryReaderInternal reader, Context context)
        {
            var length = reader.Read7BitEncodedInt();
            return length > 0 ? reader.ReadBytes(length) : new byte[0];
        }
    }
}
