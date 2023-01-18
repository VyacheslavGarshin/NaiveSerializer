using Naive.Serializer.Cogs;
using System;

namespace Naive.Serializer.Handlers
{
    internal class ULongHandler : AbstractHandler
    {
        public override HandlerType HandlerType { get; } = HandlerType.ULong;

        public ULongHandler(Type type) : base(type)
        {
        }

        public override bool Match(Type type)
        {
            return type == typeof(ulong) || type == typeof(ulong?);
        }

        public override void Write(BinaryWriterInternal writer, object obj, WriteContext context)
        {
            writer.Write((ulong)obj);
        }

        public override object Read(BinaryReaderInternal reader, ReadContext context)
        {
            return reader.ReadUInt64();
        }
    }
}
