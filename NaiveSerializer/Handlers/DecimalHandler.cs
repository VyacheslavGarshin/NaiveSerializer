using System;
using System.IO;

namespace NaiveSerializer.Handlers
{
    public class DecimalHandler : IHandler
    {
        public HandlerType HandlerType { get; } = HandlerType.Decimal;

        public bool Match(Type type)
        {
            return type == typeof(decimal) || type == typeof(decimal?);
        }

        public IHandler Create(Type type)
        {
            return null;
        }

        public void Write(BinaryWriter writer, object obj, Type type)
        {
            writer.Write((decimal)obj);
        }

        public object Read(BinaryReader reader, Type type)
        {
            return reader.ReadDecimal();
        }
    }
}
