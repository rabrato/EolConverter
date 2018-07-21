using EolConverter.Encoding;
using EolConverter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EolConverter.ByteUtils
{
    public static class EolUtils
    {
        public static bool HasAnyEol(this byte[] data)
        {
            return data.FindFirstEolByteIndex() != null;
        }

        public static int? FindFirstEolByteIndex(this byte[] data)
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

        private static int? FindFirstByteIndex(this byte[] data, byte byteToFind)
        {
            return data
                .Select((dataByte, index) => new { dataByte, index })
                .FirstOrDefault(x => x.dataByte == byteToFind)?.index;
        }
    }
}
