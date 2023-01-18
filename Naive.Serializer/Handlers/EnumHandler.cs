using Naive.Serializer.Cogs;
using System;

namespace Naive.Serializer.Handlers
{
    internal class EnumHandler : AbstractHandler
    {
        public override HandlerType HandlerType { get; } = HandlerType.Enum;

        private Type _enumType;

        public EnumHandler(Type type) : base(type) 
        {
            IsSimple = false;

            if (Type != null)
            {
                _enumType = Nullable.GetUnderlyingType(Type) ?? Type;
            }
        }

        public override bool Match(Type type)
        {
            return type.IsEnum || (Nullable.GetUnderlyingType(type)?.IsEnum ?? false);
        }

        public override void Write(BinaryWriterInternal writer, object obj, Context context)
        {
            writer.Write7BitEncodedInt((int)obj);
        }

        public override object Read(BinaryReaderInternal reader, Context context)
        {
            var value = reader.Read7BitEncodedInt();

            if (_enumType == null)
            {
                return value;
            }

            return Enum.ToObject(_enumType, value);
        }
    }
}
