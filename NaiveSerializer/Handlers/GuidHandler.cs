using System;
using System.IO;

namespace NaiveSerializer.Handlers
{
    public class GuidHandler : IHandler
    {
        public HandlerType HandlerType { get; } = HandlerType.Guid;

        public bool Match(Type type)
        {
            return type == typeof(Guid) || type == typeof(Guid?);
        }

        public IHandler Create(Type type)
        {
            return null;
        }

        public void Write(BinaryWriter writer, object obj, Type type)
        {
            writer.Write(((Guid)obj).ToByteArray());
        }

        public object Read(BinaryReader reader, Type type)
        {
            return new Guid(reader.ReadBytes(16));
        }
    }
}
