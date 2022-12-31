using System;
using System.IO;

namespace NaiveSerializer.Handlers
{
    public class StringHandler : IHandler
    {
        public HandlerType HandlerType { get; } = HandlerType.String;

        public bool Match(Type type)
        {
            return type == typeof(string);
        }

        public IHandler Create(Type type)
        {
            return null;
        }

        public void Write(BinaryWriter writer, object obj, Type type)
        {
            writer.Write((string)obj);
        }

        public object Read(BinaryReader reader, Type type)
        {
            return reader.ReadString();
        }
    }
}
