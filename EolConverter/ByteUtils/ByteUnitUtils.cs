using EolConverter.Encoding;
using System.Linq;

namespace EolConverter.ByteUtils
{
    public static class ByteUnitUtils
    {
        public static bool IsEolUnit(this byte[] unit, EncodingType encoding)
        {
            return unit.IsCRUnit(encoding) || unit.IsLFUnit(encoding);
        }

        public static bool IsCRUnit(this byte[] unit, EncodingType encoding)
        {
            return unit.IsCharUnit(ByteCode.CR, encoding);
        }

        public static bool IsLFUnit(this byte[] unit, EncodingType encoding)
        {
            return unit.IsCharUnit(ByteCode.LF, encoding);
        }

        public static byte[] ToUnits(this byte[] byteChars, EncodingType encoding)
        {
            return byteChars
                .Select(byteChar => byteChar.ToUnit(encoding))
                .Aggregate((eolByteUnit1, eolByteUnit2) => eolByteUnit1.Concat(eolByteUnit2).ToArray());
        }

        private static byte[] ToUnit(this byte byteChar, EncodingType encoding)
        {
            int numBytesPerUnit = encoding.GetNumBytesPerUnit();
            var nullBytes = Enumerable.Range(0, numBytesPerUnit - 1).Select(i => ByteCode.Null);
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
