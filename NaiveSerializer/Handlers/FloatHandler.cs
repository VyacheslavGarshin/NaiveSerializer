using System;
using System.IO;

namespace NaiveSerializer.Handlers
{
    public class FloatHandler : IHandler
    {
        public HandlerType HandlerType { get; } = HandlerType.Float;

        public bool Match(Type type)
        {
            return type == typeof(float) || type == typeof(float?);
        }

        public IHandler Create(Type type)
        {
            return null;
        }

        public void Write(BinaryWriter writer, object obj, Type type)
        {
            writer.Write((float)obj);
        }

        public object Read(BinaryReader reader, Type type)
        {
            return reader.ReadSingle();
        }
    }
}
