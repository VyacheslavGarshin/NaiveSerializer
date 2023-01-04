using System;
using System.IO;

namespace Naive.Serializer.Handlers
{
    public class BoolHandler : AbstractHandler
    {
        public override HandlerType HandlerType { get; } = HandlerType.Bool;

        public BoolHandler(Type type) : base(type)
        {
        }

        public override bool Match(Type type)
        {
            return type == typeof(bool) || type == typeof(bool?);
        }

        public override void Write(BinaryWriter writer, object obj, NaiveSerializerOptions options)
        {
            writer.Write((bool)obj);
        }

        public override object Read(BinaryReader reader, NaiveSerializerOptions options)
        {
            return reader.ReadBoolean();
        }
    }
}
