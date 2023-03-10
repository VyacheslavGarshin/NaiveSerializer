using Naive.Serializer.Cogs;
using System;

namespace Naive.Serializer.Handlers
{
    internal class DoubleHandler : AbstractHandler
    {
        public override HandlerType HandlerType { get; } = HandlerType.Double;

        public DoubleHandler(Type type) : base(type)
        {
        }

        public override bool Match(Type type)
        {
            return type == typeof(double) || type == typeof(double?);
        }

        public override void Write(BinaryWriterInternal writer, object obj, WriteContext context)
        {
            writer.Write((double)obj);
        }

        public override object Read(BinaryReaderInternal reader, ReadContext context)
        {
            return reader.ReadDouble();
        }
    }
}
