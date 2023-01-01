using Naive.Serializer;
using System;
using System.IO;

namespace Naive.Serializer.Handlers
{
    public class ShortHandler : AbstractHandler<ShortHandler>
    {
        public override HandlerType HandlerType { get; } = HandlerType.Short;

        public override bool Match(Type type)
        {
            return type == typeof(short) || type == typeof(short?);
        }

        public override void Write(BinaryWriter writer, object obj, NaiveSerializerOptions options)
        {
            writer.Write((short)obj);
        }

        public override object Read(BinaryReader reader, NaiveSerializerOptions options)
        {
            return reader.ReadInt16();
        }
    }
}
