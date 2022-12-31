using System;
using System.IO;

namespace NaiveSerializer.Handlers
{
    public class IntHandler : IHandler
    {
        public HandlerType HandlerType { get; } = HandlerType.Int;

        public bool Match(Type type)
        {
            return type == typeof(int) || type == typeof(int?);
        }

        public IHandler Create(Type type)
        {
            return null;
        }

        public void Write(BinaryWriter writer, object obj, Type type)
        {
            writer.Write((int)obj);
        }

        public object Read(BinaryReader reader, Type type)
        {
            return reader.ReadInt32();
        }
    }
}
