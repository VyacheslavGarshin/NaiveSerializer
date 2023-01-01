using Naive.Serializer;
using System;
using System.IO;

namespace Naive.Serializer.Handlers
{
    public class ByteHandler : AbstractHandler<ByteHandler>
    {
        public override HandlerType HandlerType { get; } = HandlerType.Byte;

        public override bool Match(Type type)
        {
            return type == typeof(byte) || type == typeof(byte?);
        }

        public override void Write(BinaryWriter writer, object obj, NaiveSerializerOptions options)
        {
            writer.Write((byte)obj);
        }

        public override object Read(BinaryReader reader, NaiveSerializerOptions options)
        {
            return reader.ReadByte();
        }
    }
}
