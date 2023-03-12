/**************************************************************************************************
 * BinaryWriterEx - .Net 4.8
 *      An extended version of the regular BinaryWriter that adds support for selective endianness.
 *      Expect some overhead. BinaryPrimitives in .Net Core 2.1+ has BinaryPrimitives,
 *      which is better optimized for this job.
 *      
 * Inspired by Zoltu's EndianAwareBinaryReaderWriter project:
 *      https://github.com/Zoltu/Zoltu.EndianAwareBinaryReaderWriter
 *      
 * Credits:
 *      Medstar, Zoltu
 **************************************************************************************************/

using System;
using System.Text;
using System.IO;
 
namespace Medstar.CodeSnippets
{
    public class BinaryWriterEx : BinaryWriter
    {
        public enum Endianness { Little, Big }
        private Endianness _endianness = Endianness.Little;
        private bool NeedToSwapByteOrder
        {
            get
            {
                // If source endianness and endianness of BitConverter do not match, swap the byte order
                return (_endianness == Endianness.Little && !BitConverter.IsLittleEndian) ||
                       (_endianness == Endianness.Big    &&  BitConverter.IsLittleEndian);
            }
        }

        #region Custom Functions
        public long SeekBack(long offset) => BaseStream.Seek(offset, SeekOrigin.Current);
        #endregion

        #region Overrides
        // Signed
        public override void Write(short  value) => WriteWithEndianness(BitConverter.GetBytes(value));
        public override void Write(int    value) => WriteWithEndianness(BitConverter.GetBytes(value));
        public override void Write(long   value) => WriteWithEndianness(BitConverter.GetBytes(value));

        // Unsigned
        public override void Write(ushort  value) => WriteWithEndianness(BitConverter.GetBytes(value));
        public override void Write(uint    value) => WriteWithEndianness(BitConverter.GetBytes(value));
        public override void Write(ulong   value) => WriteWithEndianness(BitConverter.GetBytes(value));

        // Precision
        public override void Write(float   value) => WriteWithEndianness(BitConverter.GetBytes(value));
        public override void Write(double  value) => WriteWithEndianness(BitConverter.GetBytes(value));
        public override void Write(decimal value) => WriteDecimalWithEndianness(value);
        #endregion

        #region Helpers
        public void ChangeEndianness(Endianness endianness) { _endianness = endianness; }

        private void WriteWithEndianness(byte[] bytes)
        {
            if (NeedToSwapByteOrder) { Array.Reverse(bytes); }
            Write(bytes);
        }

        private void WriteDecimalWithEndianness(decimal value)
        {
            int[] bits = decimal.GetBits(value);
            if (NeedToSwapByteOrder) { Array.Reverse(bits); } // Reverse array of 4 Int32's (16 bytes total)

            for (int i = 0; i < 4; i++)
                WriteWithEndianness(BitConverter.GetBytes(bits[i])); // Reverse bits of selected Int32 (4 bytes)
        }
        #endregion

        #region Constructors
        // Default Constructors (may not be needed)
        public BinaryWriterEx(Stream stream) : base(stream) { }
        public BinaryWriterEx(Stream stream, Encoding encoding) : base(stream, encoding) { }
        public BinaryWriterEx(Stream stream, Encoding encoding, bool leaveOpen) : base(stream, encoding, leaveOpen) { }

        // Extended Constructors (default parameters?)
        public BinaryWriterEx(Stream stream, Endianness endianness) : base(stream) { _endianness = endianness; }
        public BinaryWriterEx(Stream stream, Encoding encoding, Endianness endianness) : base(stream, encoding) { _endianness = endianness; }
        public BinaryWriterEx(Stream stream, Encoding encoding, bool leaveOpen, Endianness endianness) : base(stream, encoding, leaveOpen) { _endianness = endianness; }
        #endregion
    }
}