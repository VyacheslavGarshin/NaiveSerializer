using System;
using System.IO;

namespace Naive.Serializer.Handlers
{
    public class GuidHandler : AbstractHandler<GuidHandler>
    {
        public override HandlerType HandlerType { get; } = HandlerType.Guid;

        public override bool Match(Type type)
        {
            return type == typeof(Guid) || type == typeof(Guid?);
        }

        public override void Write(BinaryWriter writer, object obj, NaiveSerializerOptions options)
        {
            writer.Write(((Guid)obj).ToByteArray());
        }

        public override object Read(BinaryReader reader, Type type, NaiveSerializerOptions options)
        {
            return new Guid(reader.ReadBytes(16));
        }
    }
}
