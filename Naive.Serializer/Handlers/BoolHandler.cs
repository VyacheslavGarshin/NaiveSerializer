using Naive.Serializer.Cogs;
using System;

namespace Naive.Serializer.Handlers
{
    internal class BoolHandler : AbstractHandler
    {
        public override HandlerType HandlerType { get; } = HandlerType.Bool;

        public BoolHandler(Type type) : base(type)
        {
        }

        public override bool Match(Type type)
        {
            return type == typeof(bool) || type == typeof(bool?);
        }

        public override void Write(BinaryWriterInternal writer, object obj, Context context)
        {
            writer.Write((bool)obj);
        }

        public override object Read(BinaryReaderInternal reader, Context context)
        {
            return reader.ReadBoolean();
        }
    }
}
