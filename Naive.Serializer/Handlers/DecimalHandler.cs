using System;
using System.IO;

namespace NaiveSerializer.Handlers
{
    public class DecimalHandler : AbstractHandler<DecimalHandler>
    {
        public override HandlerType HandlerType { get; } = HandlerType.Decimal;

        public override bool Match(Type type)
        {
            return type == typeof(decimal) || type == typeof(decimal?);
        }

        public override void Write(BinaryWriter writer, object obj, NaiveSerializerOptions options)
        {
            writer.Write((decimal)obj);
        }

        public override object Read(BinaryReader reader, Type type, NaiveSerializerOptions options)
        {
            return reader.ReadDecimal();
        }
    }
}
