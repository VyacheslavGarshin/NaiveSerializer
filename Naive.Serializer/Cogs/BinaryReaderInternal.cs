using System;
using System.IO;
using System.Text;

namespace Naive.Serializer.Cogs
{
    internal class BinaryReaderInternal : BinaryReader
    {
        public BinaryReaderInternal(Stream input, Encoding encoding, bool leaveOpen) : base(input, encoding, leaveOpen)
        {
        }

        public new int Read7BitEncodedInt()
        {
            return base.Read7BitEncodedInt();
        }

        // https://github.com/microsoft/referencesource/blob/master/mscorlib/system/io/binaryreader.cs
        public long Read7BitEncodedLong()
        {
            // Read out an Int32 7 bits at a time.  The high bit
            // of the byte when on means to continue reading more bytes.
            long count = 0;
            int shift = 0;
            byte b;
            do
            {
                // Check for a corrupted stream.  Read a max of 5 bytes.
                // In a future version, add a DataFormatException.
                if (shift == 10 * 7)  // 10 bytes max per Int64, shift += 7
                    throw new FormatException("Format_Bad7BitInt32");

                // ReadByte handles end of stream cases for us.
                b = ReadByte();
                count |= (long)(b & 0x7F) << shift;
                shift += 7;
            } while ((b & 0x80) != 0);
            return count;
        }
    }
}
