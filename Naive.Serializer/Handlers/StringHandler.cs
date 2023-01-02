using System;
using System.IO;

namespace Naive.Serializer.Handlers
{
    public class StringHandler : AbstractHandler
    {
        public override HandlerType HandlerType { get; } = HandlerType.String;

        public override bool Match(Type type)
        {
            return type == typeof(string);
        }

        public override void Write(BinaryWriter writer, object obj, NaiveSerializerOptions options)
        {
            writer.Write((string)obj);
        }

        public override object Read(BinaryReader reader, NaiveSerializerOptions options)
        {
            return reader.ReadString();
        }
    }
}
