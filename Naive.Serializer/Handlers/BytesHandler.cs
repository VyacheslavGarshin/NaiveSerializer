using System;
using System.IO;

namespace Naive.Serializer.Handlers
{
    public class BytesHandler : AbstractHandler
    {
        public override HandlerType HandlerType { get; } = HandlerType.Bytes;

        private bool _isReadOnlyMemory;

        public override bool Match(Type type)
        {
            return type == typeof(byte[]) || type == typeof(ReadOnlyMemory<byte>) || type == typeof(ReadOnlyMemory<byte>?);
        }

        public override void SetType(Type type)
        {
            base.SetType(type);

            IsNullable = true; 
            IsSimple = false;

            if (type != null)
            {
                if (type == typeof(ReadOnlyMemory<byte>) || type == typeof(ReadOnlyMemory<byte>?))
                {
                    _isReadOnlyMemory = true;
                }

                if (type.IsValueType)
                {
                    var nullableStruct = Nullable.GetUnderlyingType(type);

                    if (nullableStruct != null)
                    {
                        Type = nullableStruct;
                    }
                    else
                    {
                        IsNullable = false;
                    }
                }
            }
        }

        public override void Write(BinaryWriter writer, object obj, NaiveSerializerOptions options)
        {
            if (_isReadOnlyMemory)
            {
                var rom = (ReadOnlyMemory<byte>)obj;
                writer.Write(rom.Length);

                if (rom.Length > 0)
                {
                    writer.Write(rom.Span);
                }
            }
            else
            {
                var bytes = (byte[])obj;
                writer.Write(bytes.Length);

                if (bytes.Length > 0)
                {
                    writer.Write(bytes);
                }
            }
        }

        public override object Read(BinaryReader reader, NaiveSerializerOptions options)
        {
            var length = reader.ReadInt32();
            return length > 0 ? reader.ReadBytes(length) : new byte[0];
        }
    }
}
