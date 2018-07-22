using System.Collections.Generic;
using System.Linq;

namespace EolConverter.Encoding
{
    internal class EncodingDetectorFromBom
    {
        private List<EncodingType> encodingsToCheckOrdered;

        internal EncodingDetectorFromBom()
        {
            encodingsToCheckOrdered = new List<EncodingType>()
            {
                EncodingType.Utf32BE,
                EncodingType.Utf32LE,
                EncodingType.Utf16BE,
                EncodingType.Utf16LE,
                EncodingType.Utf8
            };
        }

        /// <summary>
        /// Detects the encoding of the data if it has byte order mark.
        /// </summary>
        /// <param name="data">Data where to look for the byte order mark.</param>
        /// <param name="dataLength">Length of the data.</param>
        /// <returns>The encoding if detected.</returns>
        internal EncodingType GetEncodingFromBom(byte[] data, int dataLength)
        {
            foreach (var encoding in encodingsToCheckOrdered)
            {
                if (HasBom(data, dataLength, encoding))
                {
                    return encoding;
                }
            }

            return EncodingType.None;
        }

        private bool HasBom(byte[] data, int dataLength, EncodingType encoding)
        {
            var bom = encoding.GetByteOrderMark();
            if (data.Length < bom.Length || dataLength < bom.Length)
            {
                return false;
            }

            return data.Take(bom.Length).SequenceEqual(bom);
        }
    }
}
