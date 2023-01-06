using Naive.Serializer.Cogs;
using System;

namespace Naive.Serializer.Handlers
{
    internal class StringHandler : AbstractHandler
    {
        public override HandlerType HandlerType { get; } = HandlerType.String;

        public StringHandler(Type type) : base(type) 
        {
            IsNullable = true;
        }

        public override bool Match(Type type)
        {
            return type == typeof(string);
        }

        public override void Write(BinaryWriterInternal writer, object obj, NaiveSerializerOptions options)
        {
            writer.Write((string)obj);
        }

        public override object Read(BinaryReaderInternal reader, NaiveSerializerOptions options)
        {
            return reader.ReadString();
        }
    }
}
