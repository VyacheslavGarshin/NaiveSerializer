using System;
using System.IO;

namespace Naive.Serializer.Handlers
{
    public class NullHandler : AbstractHandler<NullHandler>
    {
        public override HandlerType HandlerType { get; } = HandlerType.Null;

        public override bool Match(Type type)
        {
            return false;
        }

        public override void Write(BinaryWriter writer, object obj, NaiveSerializerOptions options)
        {
            writer.Write((byte)(obj == null ? 0 : 1));
        }

        public override object Read(BinaryReader reader, NaiveSerializerOptions options)
        {
            return reader.ReadByte();
        }
    }
}
