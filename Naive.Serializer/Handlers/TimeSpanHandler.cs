using System;
using System.IO;

namespace Naive.Serializer.Handlers
{
    internal class TimeSpanHandler : AbstractHandler
    {
        public override HandlerType HandlerType { get; } = HandlerType.TimeSpan;

        public TimeSpanHandler(Type type) : base(type)
        {
        }

        public override bool Match(Type type)
        {
            return type == typeof(TimeSpan) || type == typeof(TimeSpan?);
        }

        public override void Write(BinaryWriter writer, object obj, NaiveSerializerOptions options)
        {
            writer.Write(((TimeSpan)obj).TotalMilliseconds);
        }

        public override object Read(BinaryReader reader, NaiveSerializerOptions options)
        {
            return TimeSpan.FromMilliseconds(reader.ReadDouble());
        }
    }
}
