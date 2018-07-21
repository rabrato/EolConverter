using EolConverter.Encoding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EolConverter.ByteUtils
{
    public static class ByteUnitUtils
    {
        public static bool IsEolUnit(this byte[] unit, EncodingType encoding)
        {
            return unit.IsCrUnit(encoding) || unit.IsLfUnit(encoding);
        }

        public static bool IsCrUnit(this byte[] unit, EncodingType encoding)
        {
            return unit.IsCharUnit(ByteCode.Cr, encoding);
        }

        public static bool IsLfUnit(this byte[] unit, EncodingType encoding)
        {
            return unit.IsCharUnit(ByteCode.Lf, encoding);
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
            var emptyBytes = Enumerable.Range(0, numBytesPerUnit - 1).Select(i => ByteCode.Empty);
            var byteCharArray = new byte[1] { byteChar };
            if (encoding.IsBigEndian())
            {
                return emptyBytes.Concat(byteCharArray).ToArray();
            }

            return byteCharArray.Concat(emptyBytes).ToArray();
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
