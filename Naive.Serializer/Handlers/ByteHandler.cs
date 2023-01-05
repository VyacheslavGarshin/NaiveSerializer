using System;
using System.IO;

namespace Naive.Serializer.Handlers
{
    internal class ByteHandler : AbstractHandler
    {
        public override HandlerType HandlerType { get; } = HandlerType.Byte;

        public ByteHandler(Type type) : base(type)
        {
        }

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
