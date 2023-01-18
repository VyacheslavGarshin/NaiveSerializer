namespace Naive.Serializer.Cogs
{
    internal class AbstractContext
    {
        public NaiveSerializerOptions Options { get; set; }

        public AbstractContext(NaiveSerializerOptions options)
        {
            Options = options;
        }
    }
}
