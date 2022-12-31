using System;
using System.IO;

namespace NaiveSerializer.Handlers
{
    public class DateTimeHandler : IHandler
    {
        public HandlerType HandlerType { get; } = HandlerType.DateTime;

        public bool Match(Type type)
        {
            return type == typeof(DateTime) || type == typeof(DateTime?);
        }

        public IHandler Create(Type type)
        {
            return null;
        }

        public void Write(BinaryWriter writer, object obj, Type type)
        {
            writer.Write(((DateTime)obj).ToBinary());
        }

        public object Read(BinaryReader reader, Type type)
        {
            return new DateTime(reader.ReadInt64());
        }
    }
}
