using EolConverter.EolConverters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EolConverter.Encoding
{
    public static class EncodingDetector
    {
        public static EncodingType GetEncoding(this byte[] data, int dataLength)
        {
            var encoding = data.GetEncodingFromBom();
            if (encoding != EncodingType.None)
            {
                return encoding;
            }

            return data.GetEncodingFromEolBytes(dataLength);
        }
    }
}
