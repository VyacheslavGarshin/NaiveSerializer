using System;
using System.IO;

namespace NaiveSerializer.Handlers
{
    public class DateTimeOffsetHandler : IHandler
    {
        public HandlerType HandlerType { get; } = HandlerType.DateTimeOffset;

        public bool Match(Type type)
        {
            return type == typeof(DateTimeOffset) || type == typeof(DateTimeOffset?);
        }

        public IHandler Create(Type type)
        {
            return null;
        }

        public void Write(BinaryWriter writer, object obj, Type type)
        {
            var value = (DateTimeOffset)obj;
            NaiveSerializer.GetHandler(HandlerType.DateTime).Write(writer, value.DateTime, typeof(DateTime));
            NaiveSerializer.GetHandler(HandlerType.TimeSpan).Write(writer, value.Offset, typeof(TimeSpan));
        }

        public object Read(BinaryReader reader, Type type)
        {
            var dateTime = (DateTime)NaiveSerializer.GetHandler(HandlerType.DateTime).Read(reader, typeof(DateTime));
            var offset = (TimeSpan)NaiveSerializer.GetHandler(HandlerType.TimeSpan).Read(reader, typeof(TimeSpan));
            return new DateTimeOffset(dateTime, offset);
        }
    }
}
