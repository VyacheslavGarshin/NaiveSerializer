using Naive.Serializer.Cogs;
using System;

namespace Naive.Serializer.Handlers
{
    internal class IntHandler : AbstractHandler
    {
        public override HandlerType HandlerType { get; } = HandlerType.Int;

        public IntHandler(Type type) : base(type)
        {
        }

        public override bool Match(Type type)
        {
            return type == typeof(int) || type == typeof(int?);
        }

        public override void Write(BinaryWriterInternal writer, object obj, Context context)
        {
            writer.Write7BitEncodedInt((int)obj);
        }

        public override object Read(BinaryReaderInternal reader, Context context)
        {
            return reader.Read7BitEncodedInt();
        }
    }
}
