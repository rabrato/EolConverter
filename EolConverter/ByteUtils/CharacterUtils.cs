using EolConverter.Encoding;
using EolConverter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EolConverter.ByteUtils
{
    public static class CharacterUtils
    {
        public static bool IsEol(this byte[] unit, EncodingType encoding)
        {
            return unit.IsCr(encoding) || unit.IsLf(encoding);
        }

        public static bool IsCr(this byte[] unit, EncodingType encoding)
        {
            return unit.IsChar(ByteCode.Cr, encoding);
        }

        public static bool IsLf(this byte[] unit, EncodingType encoding)
        {
            return unit.IsChar(ByteCode.Lf, encoding);
        }

        public static void CopyCrAt(this byte[] data, int index, EncodingType encoding)
        {
            data.CopyCharAt(ByteCode.Cr, index, encoding);
        }

        public static void CopyLfAt(this byte[] data, int index, EncodingType encoding)
        {
            data.CopyCharAt(ByteCode.Lf, index, encoding);
        }

        public static void CopyUnitAt(this byte[] data, byte[] unit, int index)
        {
            unit.CopyTo(data, index);
        }

        public static bool HasAnyEndOfLine(this byte[] data)
        {
            return data.FindFirstEnfOfLineByteIndex() != null;
        }

        public static int? FindFirstEnfOfLineByteIndex(this byte[] data)
        {
            // First find if there is any Cr in data
            int? eolByteIndex = data.FindFirstByteIndex(ByteCode.Cr);
            if (eolByteIndex == null)
            {
                // If no Cr in data then search any Lf
                eolByteIndex = data.FindFirstByteIndex(ByteCode.Lf);
            }

            // Will return null if there isn't any Cr or Lf in data
            return eolByteIndex;
        }

        public static int? FindFirstByteIndex(this byte[] data, byte byteToFind)
        {
            return data
                .Select((dataByte, index) => new { dataByte, index })
                .FirstOrDefault(x => x.dataByte == byteToFind)?.index;
        }

        private static void CopyCharAt(this byte[] data, byte byteChar, int index, EncodingType encoding)
        {
            var charUnit = GetCharUnit(byteChar, encoding);
            data.CopyUnitAt(charUnit, index);
        }

        private static bool IsChar(this byte[] data, byte byteChar, EncodingType encoding)
        {
            var character = GetCharUnit(byteChar, encoding);
            if (data.Length != character.Length)
            {
                return false;
            }

            return data.SequenceEqual(character);
        }

        public static byte[] GetCharUnit(byte byteChar, EncodingType encoding)
        {
            int numBytesPerUnit = encoding.GetNumBytesPerUnit();
            var emptyBytes = Enumerable.Range(0, numBytesPerUnit - 1).Select(i => ByteCode.Empty);
            return emptyBytes.Concat(new[] { byteChar }).ToArray();
        }
    }
}
