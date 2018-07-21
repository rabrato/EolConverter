using EolConverter.EolConverters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EolConverter.Encoding
{
    public class EncodingDetector
    {
        private EncodingDetectorFromBom detectorFromBom;
        private EncodingDetectorFromEol detectorFromEol;

        public EncodingDetector()
        {
            detectorFromBom = new EncodingDetectorFromBom();
            detectorFromEol = new EncodingDetectorFromEol();
        }

        public (EncodingType encoding, bool hasBom) GetEncoding(byte[] data, int dataLength)
        {
            if (data == null || data.Length == 0 || dataLength == 0)
            {
                return (EncodingType.None, false);
            }

            var encoding = detectorFromBom.GetEncodingFromBom(data, dataLength);
            if (encoding != EncodingType.None)
            {
                return (encoding, true);
            }

            return (detectorFromEol.GetEncodingFromEolBytes(data, dataLength), false);
        }
    }
}
