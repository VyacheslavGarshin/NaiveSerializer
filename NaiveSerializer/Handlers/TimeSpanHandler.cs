using System;
using System.IO;

namespace NaiveSerializer.Handlers
{
    public class TimeSpanHandler : IHandler
    {
        public HandlerType HandlerType { get; } = HandlerType.TimeSpan;

        public bool Match(Type type)
        {
            return type == typeof(TimeSpan) || type == typeof(TimeSpan?);
        }

        public IHandler Create(Type type)
        {
            return null;
        }

        public void Write(BinaryWriter writer, object obj, Type type)
        {
            writer.Write(((TimeSpan)obj).TotalMilliseconds);
        }

        public object Read(BinaryReader reader, Type type)
        {
            return TimeSpan.FromMilliseconds(reader.ReadDouble());
        }
    }
}
