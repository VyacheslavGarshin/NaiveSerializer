using System;
using System.IO;

namespace NaiveSerializer.Handlers
{
    public class DateTimeOffsetHandler : AbstractHandler<DateTimeOffsetHandler>
    {
        public override HandlerType HandlerType { get; } = HandlerType.DateTimeOffset;

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

        public override object Read(BinaryReader reader, Type type, NaiveSerializerOptions options)
        {
            var dateTime = (DateTime)NaiveSerializer.GetHandler(HandlerType.DateTime).Read(reader, typeof(DateTime), options);
            var offset = (TimeSpan)NaiveSerializer.GetHandler(HandlerType.TimeSpan).Read(reader, typeof(TimeSpan), options);
            return new DateTimeOffset(dateTime, offset);
        }
    }
}
