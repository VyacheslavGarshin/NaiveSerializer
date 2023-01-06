using Naive.Serializer.Cogs;
using System;

namespace Naive.Serializer.Handlers
{
    internal class DecimalHandler : AbstractHandler
    {
        public override HandlerType HandlerType { get; } = HandlerType.Decimal;

        public DecimalHandler(Type type) : base(type)
        {
        }

        public override bool Match(Type type)
        {
            return type == typeof(decimal) || type == typeof(decimal?);
        }

        public override void Write(BinaryWriterInternal writer, object obj, NaiveSerializerOptions options)
        {
            writer.Write((decimal)obj);
        }

        public override object Read(BinaryReaderInternal reader, NaiveSerializerOptions options)
        {
            return reader.ReadDecimal();
        }
    }
}
