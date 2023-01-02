﻿using System;
using System.IO;

namespace Naive.Serializer.Handlers
{
    public class EnumHandler : AbstractHandler
    {
        public override HandlerType HandlerType { get; } = HandlerType.Enum;

        private Type _enumType;

        public override bool Match(Type type)
        {
            return type.IsEnum || (Nullable.GetUnderlyingType(type)?.IsEnum ?? false);
        }

        public override void SetType(Type type)
        {
            base.SetType(type);

            IsSimple = false;

            if (Type != null)
            {
                _enumType = Nullable.GetUnderlyingType(Type) ?? Type;
            }
        }

        public override void Write(BinaryWriter writer, object obj, NaiveSerializerOptions options)
        {
            writer.Write((int)obj);
        }

        public override object Read(BinaryReader reader, NaiveSerializerOptions options)
        {
            var value = reader.ReadInt32();

            if (_enumType == null)
            {
                return value;
            }

            return Enum.ToObject(_enumType, value);
        }
    }
}
