using System;
using System.IO;

namespace NaiveSerializer.Handlers
{
    public class CharHandler : AbstractHandler<CharHandler>
    {
        public override HandlerType HandlerType { get; } = HandlerType.Char;

        public override bool Match(Type type)
        {
            return type == typeof(char) || type == typeof(char?);
        }

        public override void Write(BinaryWriter writer, object obj, NaiveSerializerOptions options)
        {
            writer.Write((char)obj);
        }

        public override object Read(BinaryReader reader, Type type, NaiveSerializerOptions options)
        {
            return reader.ReadChar();
        }
    }
}
