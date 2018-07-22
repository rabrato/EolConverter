using System.Collections.Generic;
using System.Linq;

namespace EolConverter.ByteUtils
{
    public static class EolUtils
    {
        /// <summary>
        /// Returns whether the data contains any end of line byte or not.
        /// </summary>
        /// <param name="data">Data where to look for an end of line byte.</param>
        /// <returns>True if a end of line has been found, false otherwise.</returns>
        public static bool HasAnyEol(this byte[] data)
        {
            return data.FindEolByteIndexes().Any();
        }

        /// <summary>
        /// Finds all end of line bytes in data and returns its indexes.
        /// </summary>
        /// <param name="data">Data where to look for end of line bytes.</param>
        /// <returns>Found end of line indexes.</returns>
        public static IEnumerable<int> FindEolByteIndexes(this byte[] data)
        {
            byte[] eolBytes = new byte[2] { ByteCode.CR, ByteCode.LF };
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
