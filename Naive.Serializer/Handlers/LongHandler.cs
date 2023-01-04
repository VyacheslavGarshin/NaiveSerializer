using System;
using System.IO;

namespace Naive.Serializer.Handlers
{
    public class LongHandler : AbstractHandler
    {
        public override HandlerType HandlerType { get; } = HandlerType.Long;

        public LongHandler(Type type) : base(type)
        {
        }

        public override bool Match(Type type)
        {
            return type == typeof(long) || type == typeof(long?);
        }

        public override void Write(BinaryWriter writer, object obj, NaiveSerializerOptions options)
        {
            writer.Write((long)obj);
        }

        public override object Read(BinaryReader reader, NaiveSerializerOptions options)
        {
            return reader.ReadInt64();
        }
    }
}
