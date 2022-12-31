using System;
using System.IO;

namespace NaiveSerializer.Handlers
{
    public class EnumHandler : IHandler
    {
        public HandlerType HandlerType { get; } = HandlerType.Enum;

        public bool Match(Type type)
        {
            return type.IsEnum || (Nullable.GetUnderlyingType(type)?.IsEnum ?? false);
        }

        public IHandler Create(Type type)
        {
            return null;
        }

        public void Write(BinaryWriter writer, object obj, Type type)
        {
            writer.Write((int)obj);
        }

        public object Read(BinaryReader reader, Type type)
        {
            return Enum.ToObject(Nullable.GetUnderlyingType(type) ?? type, reader.ReadInt32());
        }
    }
}
