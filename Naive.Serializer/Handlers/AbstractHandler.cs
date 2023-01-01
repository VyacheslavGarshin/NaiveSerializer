using System;
using System.IO;

namespace Naive.Serializer.Handlers
{
    public abstract class AbstractHandler<T> : IHandler
        where T : IHandler, new()
    {
        public Type Type { get; set; }

        public virtual HandlerType HandlerType => throw new NotImplementedException();

        public virtual bool Match(Type type)
        {
            throw new NotImplementedException();
        }

        public virtual void SetType(Type type)
        {
            Type = type;
        }

        public virtual void Write(BinaryWriter writer, object obj, NaiveSerializerOptions options)
        {
            throw new NotImplementedException();
        }

        public virtual object Read(BinaryReader reader, NaiveSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
