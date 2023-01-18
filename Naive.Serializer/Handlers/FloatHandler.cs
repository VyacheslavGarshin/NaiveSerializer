using Naive.Serializer.Cogs;
using System;

namespace Naive.Serializer.Handlers
{
    internal class FloatHandler : AbstractHandler
    {
        public override HandlerType HandlerType { get; } = HandlerType.Float;

        public FloatHandler(Type type) : base(type)
        {
        }

        public override bool Match(Type type)
        {
            return type == typeof(float) || type == typeof(float?);
        }

        public override void Write(BinaryWriterInternal writer, object obj, WriteContext context)
        {
            writer.Write((float)obj);
        }

        public override object Read(BinaryReaderInternal reader, ReadContext context)
        {
            return reader.ReadSingle();
        }
    }
}
