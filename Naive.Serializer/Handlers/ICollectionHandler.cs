using System;
using System.Collections;
using System.IO;
using System.Linq;

namespace Naive.Serializer.Handlers
{
    public class ICollectionHandler : AbstractHandler<ICollectionHandler>
    {
        public override HandlerType HandlerType { get; } = HandlerType.ICollection;

        public override bool Match(Type type)
        {
            return type.GetInterfaces().Any(x => x == typeof(ICollection));
        }

        public override void Write(BinaryWriter writer, object obj, NaiveSerializerOptions options)
        {
            throw new NotImplementedException();
        }

        public override object Read(BinaryReader reader, NaiveSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
