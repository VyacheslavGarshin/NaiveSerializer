using System;
using System.IO;

namespace Naive.Serializer.Handlers
{
    public class UIntHandler : AbstractHandler
    {
        public override HandlerType HandlerType { get; } = HandlerType.UInt;

        public override bool Match(Type type)
        {
            return type == typeof(uint) || type == typeof(uint?);
        }

        public override void Write(BinaryWriter writer, object obj, NaiveSerializerOptions options)
        {
            writer.Write((uint)obj);
        }

        public override object Read(BinaryReader reader, NaiveSerializerOptions options)
        {
            return reader.ReadUInt32();
        }
    }
}
