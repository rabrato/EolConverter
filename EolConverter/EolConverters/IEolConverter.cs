using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EolConverter.EolConverters
{
    internal interface IEolConverter
    {
        void Convert(byte[] data, int dataLength, byte[] outputData, out int outputLength);
    }
}
