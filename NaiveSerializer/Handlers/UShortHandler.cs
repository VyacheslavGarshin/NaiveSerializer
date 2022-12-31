using System;
using System.IO;

namespace NaiveSerializer.Handlers
{
    public class UShortHandler : IHandler
    {
        public HandlerType HandlerType { get; } = HandlerType.UShort;

        public bool Match(Type type)
        {
            return type == typeof(ushort) || type == typeof(ushort?);
        }

        public IHandler Create(Type type)
        {
            return null;
        }

        public void Write(BinaryWriter writer, object obj, Type type)
        {
            writer.Write((ushort)obj);
        }

        public object Read(BinaryReader reader, Type type)
        {
            return reader.ReadUInt16();
        }
    }
}
