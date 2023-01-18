using Naive.Serializer.Cogs;
using System;

namespace Naive.Serializer.Handlers
{
    internal class DateTimeOffsetHandler : AbstractHandler
    {
        public override HandlerType HandlerType { get; } = HandlerType.DateTimeOffset;

        public DateTimeOffsetHandler(Type type) : base(type)
        {
        }

        public override bool Match(Type type)
        {
            return type == typeof(DateTimeOffset) || type == typeof(DateTimeOffset?);
        }

        public override void Write(BinaryWriterInternal writer, object obj, WriteContext context)
        {
            var value = (DateTimeOffset)obj;
            NaiveSerializer.GetHandler(HandlerType.DateTime).Write(writer, value.DateTime, context);
            NaiveSerializer.GetHandler(HandlerType.TimeSpan).Write(writer, value.Offset, context);
        }

        public override object Read(BinaryReaderInternal reader, ReadContext context)
        {
            var dateTime = (DateTime)NaiveSerializer.GetHandler(HandlerType.DateTime).Read(reader, context);
            var offset = (TimeSpan)NaiveSerializer.GetHandler(HandlerType.TimeSpan).Read(reader, context);
            return new DateTimeOffset(dateTime, offset);
        }
    }
}
