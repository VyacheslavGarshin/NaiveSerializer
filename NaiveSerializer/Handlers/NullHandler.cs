using System;
using System.IO;

namespace NaiveSerializer.Handlers
{
    public class NullHandler : IHandler
    {
        public HandlerType HandlerType { get; } = HandlerType.Null;

        public bool Match(Type type)
        {
            return false;
        }

        public IHandler Create(Type type)
        {
            return null;
        }

        public void Write(BinaryWriter writer, object obj, Type type)
        {
            writer.Write((byte)(obj == null ? 0 : 1));
        }

        public object Read(BinaryReader reader, Type type)
        {
            return reader.ReadByte();
        }
    }
}
