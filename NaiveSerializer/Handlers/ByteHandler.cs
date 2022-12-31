using System;
using System.IO;

namespace NaiveSerializer.Handlers
{
    public class ByteHandler : IHandler
    {
        public HandlerType HandlerType { get; } = HandlerType.Byte;

        public bool Match(Type type)
        {
            return type == typeof(byte) || type == typeof(byte?);
        }

        public IHandler Create(Type type)
        {
            return null;
        }

        public void Write(BinaryWriter writer, object obj, Type type)
        {
            writer.Write((byte)obj);
        }

        public object Read(BinaryReader reader, Type type)
        {
            return reader.ReadByte();
        }
    }
}
