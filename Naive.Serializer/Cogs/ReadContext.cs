using System;

namespace Naive.Serializer.Cogs
{
    internal class ReadContext : AbstractContext, IDisposable
    {
        public byte[] NameBuffer { get; set; }

        public ReadContext(NaiveSerializerOptions options) : base(options) 
        {
            NameBuffer = options.ArrayPool.Rent(byte.MaxValue);
        }

        public void Dispose()
        {
            Options.ArrayPool.Return(NameBuffer);
        }
    }
}
