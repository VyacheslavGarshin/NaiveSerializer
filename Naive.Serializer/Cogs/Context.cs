using System;
using System.Collections.Generic;

namespace Naive.Serializer.Cogs
{
    internal class Context : IDisposable
    {
        public NaiveSerializerOptions Options { get; set; }

        public byte[] NameBuffer { get; set; }

        public Stack<object> Stack { get; set; } = new Stack<object>();

        public Context(NaiveSerializerOptions options)
        {
            Options = options;
            NameBuffer = options.ArrayPool.Rent(byte.MaxValue);
        }

        public void Dispose()
        {
            Options.ArrayPool.Return(NameBuffer);
        }
    }
}
