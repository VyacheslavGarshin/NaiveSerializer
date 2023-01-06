using Naive.Serializer.Cogs;
using System;

namespace Naive.Serializer.Handlers
{
    internal class UShortHandler : AbstractHandler
    {
        public override HandlerType HandlerType { get; } = HandlerType.UShort;

        public UShortHandler(Type type) : base(type)
        {
        }

        public override bool Match(Type type)
        {
            return type == typeof(ushort) || type == typeof(ushort?);
        }

        public override void Write(BinaryWriterInternal writer, object obj, NaiveSerializerOptions options)
        {
            writer.Write((ushort)obj);
        }

        public override object Read(BinaryReaderInternal reader, NaiveSerializerOptions options)
        {
            return reader.ReadUInt16();
        }
    }
}
