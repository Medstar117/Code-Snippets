/**************************************************************************************************
 * BinaryReaderEx - .Net 4.8
 *      An extended version of the regular BinaryReader that adds support for selective endianness.
 *      Expect some overhead. BinaryPrimitives in .Net Core 2.1+ has BinaryPrimitives,
 *      which is better optimized for this job.
 *      
 * Inspired by Zoltu's EndianAwareBinaryReaderWriter project:
 *      https://github.com/Zoltu/Zoltu.EndianAwareBinaryReaderWriter      
 *      
 * Credits:
 *      Medstar, Zoltu, all answerers here:
 *      https://stackoverflow.com/questions/8620885/c-sharp-binary-reader-in-big-endian
 **************************************************************************************************/

using System;
using System.Text;
using System.IO;
using System.Linq;

namespace Medstar.CodeSnippets
{
    public class BinaryReaderEx : BinaryReader
    {
        public enum Endianness { Little, Big }
        private Endianness _endianness = Endianness.Little;
        private bool NeedToSwapByteOrder
        {
            get
            {
                // If source endianness and endianness of BitConverter do not match, swap the byte order
                return (_endianness == Endianness.Little && !BitConverter.IsLittleEndian) ||
                       (_endianness == Endianness.Big && BitConverter.IsLittleEndian);
            }
        }

        #region Custom Functions
        public long SeekBack(long offset) => BaseStream.Seek(BaseStream.Position - offset, SeekOrigin.Begin);
        #endregion

        #region Overrides
        // Signed
        public override short ReadInt16() => BitConverter.ToInt16(ReadWithEndianness(sizeof(short)), 0);
        public override int   ReadInt32() => BitConverter.ToInt32(ReadWithEndianness(sizeof(int)),   0);
        public override long  ReadInt64() => BitConverter.ToInt64(ReadWithEndianness(sizeof(long)),  0);

        // Unsigned
        public override ushort ReadUInt16() => BitConverter.ToUInt16(ReadWithEndianness(sizeof(ushort)), 0);
        public override uint   ReadUInt32() => BitConverter.ToUInt32(ReadWithEndianness(sizeof(uint)), 0);
        public override ulong  ReadUInt64() => BitConverter.ToUInt64(ReadWithEndianness(sizeof(ulong)), 0);

        // Precision
        public override float   ReadSingle() => BitConverter.ToSingle(ReadWithEndianness(sizeof(float)),  0);
        public override double  ReadDouble() => BitConverter.ToDouble(ReadWithEndianness(sizeof(double)), 0);
        public override decimal ReadDecimal() => ReadDecimalWithEndianness();
        #endregion

        #region Helpers
        public void ChangeEndianness(Endianness endianness) { _endianness = endianness; }

        // Main Reader/Converter
        private byte[] ReadWithEndianness(int bytesToRead)
        {
            byte[] bytesRead = ReadBytes(bytesToRead);
            if (NeedToSwapByteOrder) { Array.Reverse(bytesRead); }
            return bytesRead;
        }

        private decimal ReadDecimalWithEndianness()
        {
            byte[] bytes = ReadBytes(sizeof(decimal));

            int[] bits = new int[4];
            for (int i = 0; i < 4; i++)
            {
                ArraySegment<byte> bitSegment = new ArraySegment<byte>(bytes, i * 4, 4);
                bits[i] = BitConverter.ToInt32(NeedToSwapByteOrder ? bitSegment.Reverse().ToArray() : bitSegment.ToArray(), 0);
            }

            if (NeedToSwapByteOrder) { Array.Reverse(bits); }
            return new decimal(bits);
        }
        #endregion

        #region Constructors
        // Default Constructors (may not be needed)
        public BinaryReaderEx(Stream stream) : base(stream) { }
        public BinaryReaderEx(Stream stream, Encoding encoding) : base(stream, encoding) { }
        public BinaryReaderEx(Stream stream, Encoding encoding, bool leaveOpen) : base(stream, encoding, leaveOpen) { }

        // Extended Constructors (default parameters?)
        public BinaryReaderEx(Stream stream, Endianness endianness) : base(stream) { _endianness = endianness; }
        public BinaryReaderEx(Stream stream, Encoding encoding, Endianness endianness) : base(stream, encoding) { _endianness = endianness; }
        public BinaryReaderEx(Stream stream, Encoding encoding, bool leaveOpen, Endianness endianness) : base(stream, encoding, leaveOpen) { _endianness = endianness; }
        #endregion
    }
}
