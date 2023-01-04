using System;
using System.IO;

namespace Naive.Serializer.Handlers
{
    public class IntHandler : AbstractHandler
    {
        public override HandlerType HandlerType { get; } = HandlerType.Int;

        public IntHandler(Type type) : base(type)
        {
        }

        public override bool Match(Type type)
        {
            return type == typeof(int) || type == typeof(int?);
        }

        public override void Write(BinaryWriter writer, object obj, NaiveSerializerOptions options)
        {
            writer.Write((int)obj);
        }

        public override object Read(BinaryReader reader, NaiveSerializerOptions options)
        {
            return reader.ReadInt32();
        }
    }
}
