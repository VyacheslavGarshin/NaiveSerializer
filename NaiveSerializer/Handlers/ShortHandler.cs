using System;
using System.IO;

namespace NaiveSerializer.Handlers
{
    public class ShortHandler : IHandler
    {
        public HandlerType HandlerType { get; } = HandlerType.Short;

        public bool Match(Type type)
        {
            return type == typeof(short) || type == typeof(short?);
        }

        public IHandler Create(Type type)
        {
            return null;
        }

        public void Write(BinaryWriter writer, object obj, Type type)
        {
            writer.Write((short)obj);
        }

        public object Read(BinaryReader reader, Type type)
        {
            return reader.ReadInt16();
        }
    }
}
