using Naive.Serializer.Cogs;
using System;

namespace Naive.Serializer.Handlers
{
    internal class GuidHandler : AbstractHandler
    {
        public override HandlerType HandlerType { get; } = HandlerType.Guid;

        public GuidHandler(Type type) : base(type)
        {
        }

        public override bool Match(Type type)
        {
            return type == typeof(Guid) || type == typeof(Guid?);
        }

        public override void Write(BinaryWriterInternal writer, object obj, WriteContext context)
        {
            writer.Write(((Guid)obj).ToByteArray());
        }

        public override object Read(BinaryReaderInternal reader, ReadContext context)
        {
            return new Guid(reader.ReadBytes(16));
        }
    }
}
