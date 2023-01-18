using Naive.Serializer.Cogs;
using System;

namespace Naive.Serializer.Handlers
{
    internal class UIntHandler : AbstractHandler
    {
        public override HandlerType HandlerType { get; } = HandlerType.UInt;

        public UIntHandler(Type type) : base(type)
        {
        }

        public override bool Match(Type type)
        {
            return type == typeof(uint) || type == typeof(uint?);
        }

        public override void Write(BinaryWriterInternal writer, object obj, Context context)
        {
            writer.Write((uint)obj);
        }

        public override object Read(BinaryReaderInternal reader, Context context)
        {
            return reader.ReadUInt32();
        }
    }
}
