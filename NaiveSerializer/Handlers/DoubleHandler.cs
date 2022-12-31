using System;
using System.IO;

namespace NaiveSerializer.Handlers
{
    public class DoubleHandler : IHandler
    {
        public HandlerType HandlerType { get; } = HandlerType.Double;

        public bool Match(Type type)
        {
            return type == typeof(double) || type == typeof(double?);
        }

        public IHandler Create(Type type)
        {
            return null;
        }

        public void Write(BinaryWriter writer, object obj, Type type)
        {
            writer.Write((double)obj);
        }

        public object Read(BinaryReader reader, Type type)
        {
            return reader.ReadDouble();
        }
    }
}
