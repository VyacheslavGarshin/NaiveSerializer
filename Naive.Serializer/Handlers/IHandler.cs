using System.IO;
using System;
using Naive.Serializer;

namespace Naive.Serializer.Handlers
{
    public interface IHandler
    {
        public Type WriteType { get; set; }

        public HandlerType HandlerType { get; }

        public bool Match(Type type);

        public IHandler Create(Type type);

        public void Write(BinaryWriter writer, object obj, NaiveSerializerOptions options);

        public object Read(BinaryReader reader, Type type, NaiveSerializerOptions options);
    }
}
