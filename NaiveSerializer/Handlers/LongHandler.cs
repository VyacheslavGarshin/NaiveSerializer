﻿using System;
using System.IO;

namespace NaiveSerializer.Handlers
{
    public class LongHandler : AbstractHandler<LongHandler>
    {
        public override HandlerType HandlerType { get; } = HandlerType.Long;

        public override bool Match(Type type)
        {
            return type == typeof(long) || type == typeof(long?);
        }

        public override void Write(BinaryWriter writer, object obj, NaiveSerializerOptions options)
        {
            writer.Write((long)obj);
        }

        public override object Read(BinaryReader reader, Type type, NaiveSerializerOptions options)
        {
            return reader.ReadInt64();
        }
    }
}
