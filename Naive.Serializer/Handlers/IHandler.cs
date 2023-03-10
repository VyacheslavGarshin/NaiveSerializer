using Naive.Serializer.Cogs;
using System;

namespace Naive.Serializer.Handlers
{
    internal interface IHandler
    {
        public Type Type { get; }

        public bool IsObject { get; }
        
        public bool IsNullable { get; }

        public bool IsSimple { get; }

        public HandlerType HandlerType { get; }

        public bool Match(Type type);

        public void Write(BinaryWriterInternal writer, object obj, WriteContext context);

        public object Read(BinaryReaderInternal reader, ReadContext context);
    }
}
