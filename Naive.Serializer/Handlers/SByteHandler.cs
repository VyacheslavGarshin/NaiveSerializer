using Naive.Serializer.Cogs;
using System;

namespace Naive.Serializer.Handlers
{
    internal class SByteHandler : AbstractHandler
    {
        public override HandlerType HandlerType { get; } = HandlerType.SByte;

        public SByteHandler(Type type) : base(type)
        {
        }

        public override bool Match(Type type)
        {
            return type == typeof(sbyte) || type == typeof(sbyte?);
        }

        public override void Write(BinaryWriterInternal writer, object obj, Context context)
        {
            writer.Write((sbyte)obj);
        }

        public override object Read(BinaryReaderInternal reader, Context context)
        {
            return reader.ReadSByte();
        }
    }
}
