using System;
using System.IO;

namespace Naive.Serializer.Handlers
{
    public class DecimalHandler : AbstractHandler
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

        public override object Read(BinaryReader reader, NaiveSerializerOptions options)
        {
            return reader.ReadDecimal();
        }
    }
}
