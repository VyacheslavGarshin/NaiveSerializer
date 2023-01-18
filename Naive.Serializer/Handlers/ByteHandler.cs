using Naive.Serializer.Cogs;
using System;

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

        public override void Write(BinaryWriterInternal writer, object obj, Context context)
        {
            writer.Write((byte)obj);
        }

        public override object Read(BinaryReaderInternal reader, Context context)
        {
            return reader.ReadByte();
        }
    }
}
