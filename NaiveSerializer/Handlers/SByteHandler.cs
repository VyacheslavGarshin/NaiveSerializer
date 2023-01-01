using System;
using System.IO;

namespace NaiveSerializer.Handlers
{
    public class SByteHandler : AbstractHandler<SByteHandler>
    {
        public override HandlerType HandlerType { get; } = HandlerType.SByte;

        public override bool Match(Type type)
        {
            return type == typeof(sbyte) || type == typeof(sbyte?);
        }

        public override void Write(BinaryWriter writer, object obj, NaiveSerializerOptions options)
        {
            writer.Write((sbyte)obj);
        }

        public override object Read(BinaryReader reader, Type type, NaiveSerializerOptions options)
        {
            return reader.ReadSByte();
        }
    }
}
