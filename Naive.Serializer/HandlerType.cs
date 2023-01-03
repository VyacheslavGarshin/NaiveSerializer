namespace Naive.Serializer
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>Maximum is <see cref="byte.MaxValue"/></remarks>
    public enum HandlerType
    {
        Null = 0,
        SByte = 1,
        Byte = 2,
        Short = 3,
        UShort = 4,
        Int = 5,
        UInt = 6,
        Long = 7,
        ULong = 8,
        NInt = 9,
        NUInt = 10,
        Float = 11,
        Double = 12,
        Decimal = 13,
        Bool = 14,
        Char = 15,
        String = 16,
        TimeSpan = 17,
        DateTime = 18,
        DateTimeOffset = 19,
        Guid = 20,
        Enum = 21,
        Bytes = 22,
        Chars = 23,
        IDictionary = 24,
        IEnumerable = 25,
        Object = 26,
    }
}
