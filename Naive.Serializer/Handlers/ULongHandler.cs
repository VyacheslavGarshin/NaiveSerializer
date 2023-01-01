using System;
using System.IO;

namespace Naive.Serializer.Handlers
{
    public class ULongHandler : AbstractHandler<ULongHandler>
    {
        public override HandlerType HandlerType { get; } = HandlerType.ULong;

        public override bool Match(Type type)
        {
            return type == typeof(ulong) || type == typeof(ulong?);
        }

        public override void Write(BinaryWriter writer, object obj, NaiveSerializerOptions options)
        {
            writer.Write((ulong)obj);
        }

        public override object Read(BinaryReader reader, NaiveSerializerOptions options)
        {
            return reader.ReadUInt64();
        }
    }
}
