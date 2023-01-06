using Naive.Serializer.Cogs;
using System;

namespace Naive.Serializer.Handlers
{
    internal interface IHandler
    {
        public Type Type { get; }

        public bool IsNullable { get; }

        public bool IsSimple { get; }

        public HandlerType HandlerType { get; }

        public bool Match(Type type);

        public void Write(BinaryWriterInternal writer, object obj, NaiveSerializerOptions options);

        public object Read(BinaryReaderInternal reader, NaiveSerializerOptions options);
    }
}
