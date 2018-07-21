using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EolConverter.Encoding
{
    internal class EncodingDetectorFromBom
    {
        private List<EncodingType> encodingsToCheckOrdered;

        public EncodingDetectorFromBom()
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

        private bool HasBom(byte[] data, int dataLength, EncodingType enconding)
        {
            var bom = enconding.GetByteOrderMark();
            if (data.Length < bom.Length || dataLength < bom.Length)
            {
                return false;
            }

            return data.Take(bom.Length).SequenceEqual(bom);
        }
    }
}
