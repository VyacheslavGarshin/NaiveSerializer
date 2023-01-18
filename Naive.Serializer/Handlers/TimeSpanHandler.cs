using Naive.Serializer.Cogs;
using System;

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

        public override void Write(BinaryWriterInternal writer, object obj, Context context)
        {
            writer.Write(((TimeSpan)obj).TotalMilliseconds);
        }

        public override object Read(BinaryReaderInternal reader, Context context)
        {
            return TimeSpan.FromMilliseconds(reader.ReadDouble());
        }
    }
}
