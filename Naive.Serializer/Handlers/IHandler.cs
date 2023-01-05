using System;
using System.IO;

namespace Naive.Serializer.Handlers
{
    internal interface IHandler
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
