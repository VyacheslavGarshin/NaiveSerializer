using Naive.Serializer;
using System;
using System.IO;

namespace Naive.Serializer.Handlers
{
    public class DoubleHandler : AbstractHandler<DoubleHandler>
    {
        public override HandlerType HandlerType { get; } = HandlerType.Double;

        public override bool Match(Type type)
        {
            return type == typeof(double) || type == typeof(double?);
        }

        public override void Write(BinaryWriter writer, object obj, NaiveSerializerOptions options)
        {
            writer.Write((double)obj);
        }

        public override object Read(BinaryReader reader, Type type, NaiveSerializerOptions options)
        {
            return reader.ReadDouble();
        }
    }
}
