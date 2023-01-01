using System;
using System.IO;

namespace NaiveSerializer.Handlers
{
    public abstract class AbstractHandler<T> : IHandler
        where T : IHandler, new()
    {
        public Type WriteType { get; set; }

        public virtual HandlerType HandlerType => throw new NotImplementedException();

        public virtual bool Match(Type type)
        {
            throw new NotImplementedException();
        }

        public virtual IHandler Create(Type type)
        {
            return new T { WriteType = type };
        }

        public virtual void Write(BinaryWriter writer, object obj, NaiveSerializerOptions options)
        {
            throw new NotImplementedException();
        }

        public virtual object Read(BinaryReader reader, Type type, NaiveSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
