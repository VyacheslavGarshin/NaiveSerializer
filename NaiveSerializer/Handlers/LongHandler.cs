using System;
using System.IO;

namespace NaiveSerializer.Handlers
{
    public class LongHandler : IHandler
    {
        public HandlerType HandlerType { get; } = HandlerType.Long;

        public bool Match(Type type)
        {
            return type == typeof(long) || type == typeof(long?);
        }

        public IHandler Create(Type type)
        {
            return null;
        }

        public void Write(BinaryWriter writer, object obj, Type type)
        {
            writer.Write((long)obj);
        }

        public object Read(BinaryReader reader, Type type)
        {
            return reader.ReadInt64();
        }
    }
}
