using System;
using System.IO;

namespace Naive.Serializer.Handlers
{
    public class EnumHandler : AbstractHandler
    {
        public override HandlerType HandlerType { get; } = HandlerType.Enum;

        public override bool Match(Type type)
        {
            return type.IsEnum || (Nullable.GetUnderlyingType(type)?.IsEnum ?? false);
        }

        public override void Write(BinaryWriter writer, object obj, NaiveSerializerOptions options)
        {
            writer.Write((int)obj);
        }

        public override object Read(BinaryReader reader, NaiveSerializerOptions options)
        {
            var value = reader.ReadInt32();

            if (Type == null)
            {
                return value;
            }

            var enumType = Nullable.GetUnderlyingType(Type) ?? Type;

            if (enumType == null || !enumType.IsEnum)
            {
                return value;
            }

            return Enum.ToObject(enumType, value);
        }
    }
}
