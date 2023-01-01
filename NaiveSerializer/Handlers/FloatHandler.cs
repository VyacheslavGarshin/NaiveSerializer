using System;
using System.IO;

namespace NaiveSerializer.Handlers
{
    public class FloatHandler : AbstractHandler<FloatHandler>
    {
        public override HandlerType HandlerType { get; } = HandlerType.Float;

        public override bool Match(Type type)
        {
            return type == typeof(float) || type == typeof(float?);
        }

        public override void Write(BinaryWriter writer, object obj, NaiveSerializerOptions options)
        {
            writer.Write((float)obj);
        }

        public override object Read(BinaryReader reader, Type type, NaiveSerializerOptions options)
        {
            return reader.ReadSingle();
        }
    }
}
