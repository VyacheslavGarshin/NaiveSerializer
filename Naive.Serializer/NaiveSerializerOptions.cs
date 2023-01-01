namespace Naive.Serializer
{
    public class NaiveSerializerOptions
    {
        /// <summary>
        /// Ignore missing member on deserialize.
        /// </summary>
        /// <remarks>Default is true.</remarks>
        public bool IgnoreMissingMember { get; set; } = true;

        /// <summary>
        /// Ignore null values in object properties.
        /// </summary>
        /// /// <remarks>Default is false.</remarks>
        public bool IgnoreNullValue { get; set; }
    }
}
