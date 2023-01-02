﻿using System;
using System.IO;

namespace Naive.Serializer.Handlers
{
    public class DateTimeHandler : AbstractHandler
    {
        public override HandlerType HandlerType { get; } = HandlerType.DateTime;

        public override bool Match(Type type)
        {
            return type == typeof(DateTime) || type == typeof(DateTime?);
        }

        public override void Write(BinaryWriter writer, object obj, NaiveSerializerOptions options)
        {
            writer.Write(((DateTime)obj).ToBinary());
        }

        public override object Read(BinaryReader reader, NaiveSerializerOptions options)
        {
            return new DateTime(reader.ReadInt64());
        }
    }
}
