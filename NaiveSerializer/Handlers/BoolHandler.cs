using System;
using System.IO;

namespace NaiveSerializer.Handlers
{
    public class BoolHandler : IHandler
    {
        public HandlerType HandlerType { get; } = HandlerType.Bool;

        public bool Match(Type type)
        {
            return type == typeof(bool) || type == typeof(bool?);
        }

        public IHandler Create(Type type)
        {
            return null;
        }

        public void Write(BinaryWriter writer, object obj, Type type)
        {
            writer.Write((bool)obj);
        }

        public object Read(BinaryReader reader, Type type)
        {
            return reader.ReadBoolean();
        }
    }
}
