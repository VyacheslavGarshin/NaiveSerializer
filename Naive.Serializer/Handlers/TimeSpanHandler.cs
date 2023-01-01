using Naive.Serializer;
using System;
using System.IO;

namespace Naive.Serializer.Handlers
{
    public class TimeSpanHandler : AbstractHandler<TimeSpanHandler>
    {
        public override HandlerType HandlerType { get; } = HandlerType.TimeSpan;

        public override bool Match(Type type)
        {
            return type == typeof(TimeSpan) || type == typeof(TimeSpan?);
        }

        public override void Write(BinaryWriter writer, object obj, NaiveSerializerOptions options)
        {
            writer.Write(((TimeSpan)obj).TotalMilliseconds);
        }

        public override object Read(BinaryReader reader, Type type, NaiveSerializerOptions options)
        {
            return TimeSpan.FromMilliseconds(reader.ReadDouble());
        }
    }
}
