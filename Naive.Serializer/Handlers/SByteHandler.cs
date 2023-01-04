using System;
using System.IO;

namespace Naive.Serializer.Handlers
{
    public class SByteHandler : AbstractHandler
    {
        public override HandlerType HandlerType { get; } = HandlerType.SByte;

        public SByteHandler(Type type) : base(type)
        {
        }

        public override bool Match(Type type)
        {
            return type == typeof(sbyte) || type == typeof(sbyte?);
        }

        public override void Write(BinaryWriter writer, object obj, NaiveSerializerOptions options)
        {
            writer.Write((sbyte)obj);
        }

        public override object Read(BinaryReader reader, NaiveSerializerOptions options)
        {
            return reader.ReadSByte();
        }
    }
}
