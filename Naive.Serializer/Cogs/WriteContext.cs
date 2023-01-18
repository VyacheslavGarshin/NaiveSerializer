using System.Collections.Generic;

namespace Naive.Serializer.Cogs
{
    internal class WriteContext : AbstractContext
    {
        public Stack<object> Stack { get; set; } = new Stack<object>();

        public int Depth { get; set; }

        public WriteContext(NaiveSerializerOptions options) : base(options)
        {
            Options = options;
        }
    }
}
