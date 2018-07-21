using EolConverter.Encoding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EolConverter.EolConverters
{
    internal class CrEolConverter : IEolConverter
    {
        EncodingDetector encodingDetector;

        public CrEolConverter(EncodingDetector encodingDetector)
        {
            this.encodingDetector = encodingDetector;
        }

        public void Convert(byte[] data, int dataLength, byte[] outputData, out int outputLength)
        {
            
        }
    }
}
