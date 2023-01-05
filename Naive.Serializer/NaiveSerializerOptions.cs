using System.Buffers;

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
        /// <remarks>Default is false.</remarks>
        public bool IgnoreNullValue { get; set; }

        /// <summary>
        /// Automatically create unique index on object properties. Possibly not full.
        /// </summary>
        /// <remarks>
        /// If field is not found in index than search in dictionary. Default is true. 
        /// Maximum speed gain on deserialization 24%.
        /// Default is true.
        /// </remarks>
        public bool UsePropertiesIndex { get; set; } = true;

        /// <summary>
        /// If properties index is full than compare only index values, do not compare by property name values.
        /// </summary>
        /// <remarks>
        /// Possible error if payload contains a lot more fields than the deserialized class has.
        /// Speed gain on deserialization 9%.
        /// Default is false.
        /// </remarks>
        public bool OptimisticIndexSearch { get; set; } = false;

        /// <summary>
        /// Array pool used for various buffers.
        /// </summary>
        /// <remarks>Shared by default.</remarks>
        public ArrayPool<byte> ArrayPool { get; set; } = ArrayPool<byte>.Shared;
    }
}
