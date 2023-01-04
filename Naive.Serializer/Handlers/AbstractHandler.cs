using System;
using System.IO;

namespace Naive.Serializer.Handlers
{
    public abstract class AbstractHandler : IHandler
    {
        public Type Type { get; protected set; }

        public abstract HandlerType HandlerType { get; }

        public bool IsNullable { get; protected set; }

        public bool IsSimple { get; protected set; } = true;

        public abstract bool Match(Type type);

        public AbstractHandler(Type type)
        {
            Type = type;

            if (Type != null)
            {
                IsNullable = Nullable.GetUnderlyingType(Type) != null;
            }
        }

        public abstract void Write(BinaryWriter writer, object obj, NaiveSerializerOptions options);

        public abstract object Read(BinaryReader reader, NaiveSerializerOptions options);
    }
}
