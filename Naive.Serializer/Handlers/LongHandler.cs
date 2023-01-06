﻿using Naive.Serializer.Cogs;
using System;

namespace Naive.Serializer.Handlers
{
    internal class LongHandler : AbstractHandler
    {
        public override HandlerType HandlerType { get; } = HandlerType.Long;

        public LongHandler(Type type) : base(type)
        {
        }

        public override bool Match(Type type)
        {
            return type == typeof(long) || type == typeof(long?);
        }

        public override void Write(BinaryWriterInternal writer, object obj, NaiveSerializerOptions options)
        {
            writer.Write7BitEncodedLong((long)obj);
        }

        public override object Read(BinaryReaderInternal reader, NaiveSerializerOptions options)
        {
            return reader.Read7BitEncodedLong();
        }
    }
}
