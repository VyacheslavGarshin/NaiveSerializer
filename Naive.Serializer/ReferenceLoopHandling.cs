namespace Naive.Serializer
{
    /// <summary>
    /// Reference loop handling option.
    /// </summary>
    public enum ReferenceLoopHandling
    {
        /// <summary>
        /// Throw a ArgumentException when a referenct loop is found.
        /// </summary>
        Error = 0,

        /// <summary>
        /// Ignore loop references and serialize object as null.
        /// </summary>
        Ignore = 1,

        /// <summary>
        /// Do not check loop reference. 
        /// </summary>
        Serialize = 2
    }
}
