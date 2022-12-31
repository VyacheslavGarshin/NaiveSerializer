using System;
using System.IO;

namespace NaiveSerializer.Handlers
{
    public class CharHandler : IHandler
    {
        public HandlerType HandlerType { get; } = HandlerType.Char;

        public bool Match(Type type)
        {
            return type == typeof(char) || type == typeof(char?);
        }

        public IHandler Create(Type type)
        {
            return null;
        }

        public void Write(BinaryWriter writer, object obj, Type type)
        {
            writer.Write((char)obj);
        }

        public object Read(BinaryReader reader, Type type)
        {
            return reader.ReadChar();
        }
    }
}
