using Naive.Serializer;
using System;
using System.Collections;
using System.IO;
using System.Linq;

namespace Naive.Serializer.Handlers
{
    public class IEnumerableHandler : AbstractHandler<IEnumerableHandler>
    {
        public override HandlerType HandlerType { get; } = HandlerType.IEnumerable;

        public override bool Match(Type type)
        {
            return type.GetInterfaces().Any(x => x == typeof(IEnumerable));
        }

        public override void Write(BinaryWriter writer, object obj, NaiveSerializerOptions options)
        {
            throw new NotImplementedException();
        }

        public override object Read(BinaryReader reader, Type type, NaiveSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
