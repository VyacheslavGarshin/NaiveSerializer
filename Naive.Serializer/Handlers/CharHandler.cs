using Naive.Serializer.Cogs;
using System;

namespace Naive.Serializer.Handlers
{
    internal class CharHandler : AbstractHandler
    {
        public override HandlerType HandlerType { get; } = HandlerType.Char;

        public CharHandler(Type type) : base(type)
        {
        }

        public override bool Match(Type type)
        {
            return type == typeof(char) || type == typeof(char?);
        }

        public override void Write(BinaryWriterInternal writer, object obj, Context context)
        {
            writer.Write((char)obj);
        }

        public override object Read(BinaryReaderInternal reader, Context context)
        {
            return reader.ReadChar();
        }
    }
}
