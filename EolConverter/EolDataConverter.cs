using EolConverter.Encoding;
using EolConverter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EolConverter.ByteUtils;

namespace EolConverter
{
    public class EolDataConverter
    {
        EolConverter eolConverter;

        public EolDataConverter(EolConversion eolConversion)
        {
            eolConverter = new EolConverter(eolConversion, new EncodingDetector());
        }

        public void Convert(byte[] data, int dataLength, byte[] outputData, out int outputLength)
        {
            eolConverter.Convert(data, dataLength, outputData, out outputLength);
        }
    }
}
