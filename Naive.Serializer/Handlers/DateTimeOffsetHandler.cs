using System;
using System.IO;

namespace Naive.Serializer.Handlers
{
    public class DateTimeOffsetHandler : AbstractHandler
    {
        public override HandlerType HandlerType { get; } = HandlerType.DateTimeOffset;

        public DateTimeOffsetHandler(Type type) : base(type)
        {
        }

        public override bool Match(Type type)
        {
            return type == typeof(DateTimeOffset) || type == typeof(DateTimeOffset?);
        }

        public override void Write(BinaryWriter writer, object obj, NaiveSerializerOptions options)
        {
            var value = (DateTimeOffset)obj;
            NaiveSerializer.GetHandler(HandlerType.DateTime).Write(writer, value.DateTime, options);
            NaiveSerializer.GetHandler(HandlerType.TimeSpan).Write(writer, value.Offset, options);
        }

        public override object Read(BinaryReader reader, NaiveSerializerOptions options)
        {
            var dateTime = (DateTime)NaiveSerializer.GetHandler(HandlerType.DateTime).Read(reader, options);
            var offset = (TimeSpan)NaiveSerializer.GetHandler(HandlerType.TimeSpan).Read(reader, options);
            return new DateTimeOffset(dateTime, offset);
        }
    }
}
