using System.IO;
using System;

namespace Naive.Serializer.Handlers
{
    public interface IHandler
    {
        public Type Type { get; }

        public bool IsNullable { get; }

        public bool IsSimple { get; }

        public HandlerType HandlerType { get; }

        public bool Match(Type type);

        public void Write(BinaryWriter writer, object obj, NaiveSerializerOptions options);

        public object Read(BinaryReader reader, NaiveSerializerOptions options);
    }
}
