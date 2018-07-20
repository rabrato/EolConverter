using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EolConverter.EolConverters
{
    internal class NullEolConverter : IEolConverter
    {
        public void Convert(byte[] data, int dataLength, byte[] outputData, out int outputLength)
        {
            outputData = data;
            outputLength = dataLength;
        }
    }
}
