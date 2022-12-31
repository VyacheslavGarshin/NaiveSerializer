using System;
using System.IO;

namespace NaiveSerializer.Handlers
{
    public class ULongHandler : IHandler
    {
        public HandlerType HandlerType { get; } = HandlerType.ULong;

        public bool Match(Type type)
        {
            return type == typeof(ulong) || type == typeof(ulong?);
        }

        public IHandler Create(Type type)
        {
            return null;
        }

        public void Write(BinaryWriter writer, object obj, Type type)
        {
            writer.Write((ulong)obj);
        }

        public object Read(BinaryReader reader, Type type)
        {
            return reader.ReadUInt64();
        }
    }
}
