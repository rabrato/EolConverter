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
            return data.FindEolByteIndexes().Any();
        }

        public static IEnumerable<int> FindEolByteIndexes(this byte[] data)
        {
            byte[] eolBytes = new byte[2] { ByteCode.Cr, ByteCode.Lf };
            return eolBytes.SelectMany(b => data.FindByteIndexes(b));
        }

        private static IEnumerable<int> FindByteIndexes(this byte[] data, byte byteToFind)
        {
            return data
                .Select((dataByte, index) => new { dataByte, index })
                .Where(x => x.dataByte == byteToFind)
                .Select(x => x.index);
        }
    }
}
