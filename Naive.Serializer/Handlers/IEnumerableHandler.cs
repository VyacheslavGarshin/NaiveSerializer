using System;
using System.Collections;
using System.IO;
using System.Linq;

namespace Naive.Serializer.Handlers
{
    public class IEnumerableHandler : AbstractHandler
    {
        public override HandlerType HandlerType { get; } = HandlerType.IEnumerable;

        public override bool Match(Type type)
        {
            return type.GetInterfaces().Any(x => x == typeof(IEnumerable));
        }

        public override void SetType(Type type)
        {
            base.SetType(type);

            IsNullable = true;
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
