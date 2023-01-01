using System;
using System.IO;

namespace NaiveSerializer.Handlers
{
    public class BoolHandler : AbstractHandler<BoolHandler>
    {
        public override HandlerType HandlerType { get; } = HandlerType.Bool;

        public override bool Match(Type type)
        {
            return type == typeof(bool) || type == typeof(bool?);
        }

        public override void Write(BinaryWriter writer, object obj, NaiveSerializerOptions options)
        {
            writer.Write((bool)obj);
        }

        public override object Read(BinaryReader reader, Type type, NaiveSerializerOptions options)
        {
            return reader.ReadBoolean();
        }
    }
}
