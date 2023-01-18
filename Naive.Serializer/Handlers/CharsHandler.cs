using Naive.Serializer.Cogs;
using System;

namespace Naive.Serializer.Handlers
{
    internal class CharsHandler : AbstractHandler
    {
        public override HandlerType HandlerType { get; } = HandlerType.Chars;

        public override bool Match(Type type)
        {
            return type == typeof(char[]);
        }

        public CharsHandler(Type type) : base(type)
        {
            IsNullable = true;
        }

        public override void Write(BinaryWriterInternal writer, object obj, Context context)
        {
            var chars = (char[])obj;
            writer.Write7BitEncodedInt(chars.Length);

            if (chars.Length > 0)
            {
                writer.Write(chars);
            }
        }

        public override object Read(BinaryReaderInternal reader, Context context)
        {
            var length = reader.Read7BitEncodedInt();
            return length > 0 ? reader.ReadChars(length) : new char[0];
        }
    }
}
