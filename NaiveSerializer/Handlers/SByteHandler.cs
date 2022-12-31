using System;
using System.IO;

namespace NaiveSerializer.Handlers
{
    public class SByteHandler : IHandler
    {
        public HandlerType HandlerType { get; } = HandlerType.SByte;

        public bool Match(Type type)
        {
            return type == typeof(sbyte) || type == typeof(sbyte?);
        }

        public IHandler Create(Type type)
        {
            return null;
        }

        public void Write(BinaryWriter writer, object obj, Type type)
        {
            writer.Write((sbyte)obj);
        }

        public object Read(BinaryReader reader, Type type)
        {
            return reader.ReadSByte();
        }
    }
}
