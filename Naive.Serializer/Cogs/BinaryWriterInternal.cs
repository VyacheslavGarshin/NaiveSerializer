using System.IO;
using System.Text;

namespace Naive.Serializer.Cogs
{
    internal class BinaryWriterInternal : BinaryWriter
    {
        public BinaryWriterInternal(Stream output, Encoding encoding, bool leaveOpen) : base(output, encoding, leaveOpen)
        {
        }

        public new void Write7BitEncodedInt(int value)
        {
            base.Write7BitEncodedInt(value);
        }

        // https://github.com/microsoft/referencesource/blob/master/mscorlib/system/io/binarywriter.cs
        public void Write7BitEncodedLong(long value)
        {
            // Write out an int 7 bits at a time.  The high bit of the byte,
            // when on, tells reader to continue reading more bytes.
            ulong v = (ulong)value;   // support negative numbers
            while (v >= 0x80)
            {
                Write((byte)(v | 0x80));
                v >>= 7;
            }
            Write((byte)v);
        }
    }
}
