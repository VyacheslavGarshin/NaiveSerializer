using System;
using System.IO;

namespace NaiveSerializer.Handlers
{
    public class UIntHandler : IHandler
    {
        public HandlerType HandlerType { get; } = HandlerType.UInt;

        public bool Match(Type type)
        {
            return type == typeof(uint) || type == typeof(uint?);
        }

        public IHandler Create(Type type)
        {
            return null;
        }

        public void Write(BinaryWriter writer, object obj, Type type)
        {
            writer.Write((uint)obj);
        }

        public object Read(BinaryReader reader, Type type)
        {
            return reader.ReadUInt32();
        }
    }
}
