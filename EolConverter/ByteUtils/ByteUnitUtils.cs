using EolConverter.Encoding;
using System.Linq;

namespace EolConverter.ByteUtils
{
    public static class ByteUnitUtils
    {
        public static bool IsEol(this byte[] unit, EncodingType encoding)
        {
            return unit.IsCR(encoding) || unit.IsLF(encoding);
        }

        public static bool IsCR(this byte[] unit, EncodingType encoding)
        {
            return unit.IsCharUnit(ByteCode.CR, encoding);
        }

        public static bool IsLF(this byte[] unit, EncodingType encoding)
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
