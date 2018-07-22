using EolConverter.Encoding;
using System.Linq;

namespace EolConverter.ByteUtils
{
    public static class ByteUnitUtils
    {
        /// <summary>
        /// Returns if the unit is an end of line.
        /// </summary>
        /// <param name="unit">Data that will be checked.</param>
        /// <param name="encoding">The encoding of the unit.</param>
        /// <returns>True if the unit is end of line, false otherwise.</returns>
        public static bool IsEol(this byte[] unit, EncodingType encoding)
        {
            return unit.IsCR(encoding) || unit.IsLF(encoding);
        }

        /// <summary>
        /// Returns if the unit is a carriage return.
        /// </summary>
        /// <param name="unit">Data that will be checked.</param>
        /// <param name="encoding">The encoding of the unit.</param>
        /// <returns>True if the unit is a carriage return, false otherwise.</returns>
        public static bool IsCR(this byte[] unit, EncodingType encoding)
        {
            return unit.IsCharUnit(ByteCode.CR, encoding);
        }

        /// <summary>
        /// Returns if the unit is a line feed.
        /// </summary>
        /// <param name="unit">Data that will be checked.</param>
        /// <param name="encoding">The encoding of the unit.</param>
        /// <returns>True if the unit is a line feed, false otherwise.</returns>
        public static bool IsLF(this byte[] unit, EncodingType encoding)
        {
            return unit.IsCharUnit(ByteCode.LF, encoding);
        }

        /// <summary>
        /// Converts a set of bytes to a set of units using the specified encoding.
        /// </summary>
        /// <param name="byteChars">The set of bytes to be converted.</param>
        /// <param name="encoding">The encoding used to create the units.</param>
        /// <returns>A set of units.</returns>
        public static byte[] ToUnits(this byte[] byteChars, EncodingType encoding)
        {
            return byteChars
                .Select(byteChar => byteChar.ToUnit(encoding))
                .Aggregate((eolByteUnit1, eolByteUnit2) => eolByteUnit1.Concat(eolByteUnit2).ToArray());
        }

        private static byte[] ToUnit(this byte byteChar, EncodingType encoding)
        {
            var nullBytes = Enumerable.Range(0, encoding.GetNumBytesPerUnit() - 1)
                .Select(i => ByteCode.Null);
            var byteCharArray = new byte[1] { byteChar };
            if (encoding.IsBigEndian())
            {
                return nullBytes.Concat(byteCharArray).ToArray();
            }

            return byteCharArray.Concat(nullBytes).ToArray();
        }

        private static bool IsCharUnit(this byte[] data, byte byteChar, EncodingType encoding)
        {
            var charUnit = byteChar.ToUnit(encoding);
            if (data.Length != charUnit.Length)
            {
                return false;
            }

            return data.SequenceEqual(charUnit);
        }
    }
}
