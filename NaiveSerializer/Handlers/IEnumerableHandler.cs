using System;
using System.Collections;
using System.IO;
using System.Linq;

namespace NaiveSerializer.Handlers
{
    public class IEnumerableHandler : IHandler
    {
        public HandlerType HandlerType { get; } = HandlerType.IEnumerable;

        public bool Match(Type type)
        {
            return type.GetInterfaces().Any(x => x == typeof(IEnumerable));
        }

        public IHandler Create(Type type)
        {
            return null;
        }

        public void Write(BinaryWriter writer, object obj, Type type)
        {
            throw new NotImplementedException();
        }

        public object Read(BinaryReader reader, Type type)
        {
            throw new NotImplementedException();
        }
    }
}
