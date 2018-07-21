using EolConverter.Encoding;
using EolConverter.EolConverters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EolConverter
{
    public static class CharacterUtils
    {
        public static bool IsCr(this byte[] data, EncodingType encoding)
        {
            return data.IsChar(EolByte.Cr, encoding);
        }

        public static bool IsLf(this byte[] data, EncodingType encoding)
        {
            return data.IsChar(EolByte.Lf, encoding);
        }

        public static void CopyCrAt(this byte[] data, int index, EncodingType encoding)
        {
            data.CopyCharAt(EolByte.Cr, index, encoding);
        }

        public static void CopyLfAt(this byte[] data, int index, EncodingType encoding)
        {
            data.CopyCharAt(EolByte.Lf, index, encoding);
        }

        public static void CopyCharAt(this byte[] data, byte[] character, int index)
        {
            character.CopyTo(data, index);
        }

        public static bool HasAnyEndOfLine(this byte[] data)
        {
            return data.FindFirstEnfOfLineByteIndex() != null;
        }

        public static int? FindFirstEnfOfLineByteIndex(this byte[] data)
        {
            // First find if there is any Cr in data
            int? eolByteIndex = data.FindFirstByteIndex(EolByte.Cr);
            if (eolByteIndex == null)
            {
                // If no Cr in data then search any Lf
                eolByteIndex = data.FindFirstByteIndex(EolByte.Lf);
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
            var character = GetChar(byteChar, encoding);
            data.CopyCharAt(character, index);
        }

        private static bool IsChar(this byte[] data, byte byteChar, EncodingType encoding)
        {
            var character = GetChar(byteChar, encoding);
            if (data.Length != character.Length)
            {
                return false;
            }

            return data.SequenceEqual(character);
        }

        private static byte[] GetChar(byte byteChar, EncodingType encoding)
        {
            int numBytesPerUnit = encoding.GetNumBytesPerUnit();
            var emptyBytes = Enumerable.Range(0, numBytesPerUnit - 1).Select(i => EolByte.Empty);
            return emptyBytes.Concat(new[] { byteChar }).ToArray();
        }
    }
}
