using System;
using System.IO;

namespace Naive.Serializer.Handlers
{
    internal class FloatHandler : AbstractHandler
    {
        public override HandlerType HandlerType { get; } = HandlerType.Float;

        public FloatHandler(Type type) : base(type)
        {
        }

        public override bool Match(Type type)
        {
            return type == typeof(float) || type == typeof(float?);
        }

        public override void Write(BinaryWriter writer, object obj, NaiveSerializerOptions options)
        {
            writer.Write((float)obj);
        }

        public override object Read(BinaryReader reader, NaiveSerializerOptions options)
        {
            return reader.ReadSingle();
        }
    }
}
