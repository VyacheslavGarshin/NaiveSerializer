using System;
using System.IO;

namespace Naive.Serializer.Handlers
{
    public class CharsHandler : AbstractHandler
    {
        public override HandlerType HandlerType { get; } = HandlerType.Chars;

        public override bool Match(Type type)
        {
            return type == typeof(char[]);
        }

        public override void SetType(Type type)
        {
            base.SetType(type);

            IsNullable = true; 
        }

        public override void Write(BinaryWriter writer, object obj, NaiveSerializerOptions options)
        {
            var chars = (char[])obj;
            writer.Write(chars.Length);

            if (chars.Length > 0)
            {
                writer.Write(chars);
            }
        }

        public override object Read(BinaryReader reader, NaiveSerializerOptions options)
        {
            var length = reader.ReadInt32();
            return length > 0 ? reader.ReadChars(length) : new char[0];
        }
    }
}
