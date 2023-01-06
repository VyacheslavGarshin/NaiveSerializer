using Naive.Serializer.Cogs;
using System;

namespace Naive.Serializer.Handlers
{
    internal class ShortHandler : AbstractHandler
    {
        public override HandlerType HandlerType { get; } = HandlerType.Short;

        public ShortHandler(Type type) : base(type)
        {
        }

        public override bool Match(Type type)
        {
            return type == typeof(short) || type == typeof(short?);
        }

        public override void Write(BinaryWriterInternal writer, object obj, NaiveSerializerOptions options)
        {
            writer.Write((short)obj);
        }

        public override object Read(BinaryReaderInternal reader, NaiveSerializerOptions options)
        {
            return reader.ReadInt16();
        }
    }
}
