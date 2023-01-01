﻿using System;
using System.IO;

namespace NaiveSerializer.Handlers
{
    public class UShortHandler : AbstractHandler<UShortHandler>
    {
        public override HandlerType HandlerType { get; } = HandlerType.UShort;

        public override bool Match(Type type)
        {
            return type == typeof(ushort) || type == typeof(ushort?);
        }

        public override void Write(BinaryWriter writer, object obj, NaiveSerializerOptions options)
        {
            writer.Write((ushort)obj);
        }

        public override object Read(BinaryReader reader, Type type, NaiveSerializerOptions options)
        {
            return reader.ReadUInt16();
        }
    }
}
