using Naive.Serializer.Cogs;
using System;

namespace Naive.Serializer.Handlers
{
    internal abstract class AbstractHandler : IHandler
    {
        public Type Type { get; protected set; }

        public abstract HandlerType HandlerType { get; }

        public bool IsNullable { get; protected set; }

        public bool IsSimple { get; protected set; } = true;

        public bool IsObject { get; protected set; }

        public abstract bool Match(Type type);

        public AbstractHandler(Type type)
        {
            Type = type;

            if (Type != null)
            {
                IsNullable = Nullable.GetUnderlyingType(Type) != null;
            }
        }

        public abstract void Write(BinaryWriterInternal writer, object obj, WriteContext context);

        public abstract object Read(BinaryReaderInternal reader, ReadContext context);
    }
}
