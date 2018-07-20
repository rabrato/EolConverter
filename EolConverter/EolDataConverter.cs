using EolConverter.EolConverters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EolConverter
{
    public class EolDataConverter
    {
        IEolConverter eolConverter;

        public EolDataConverter(EolConversion eolConversion)
        {
            eolConverter = EolConverterFactory.GetEolConverter(eolConversion);
        }

        public void Convert(byte[] data, int dataLength, byte[] outputData, out int outputLength)
        {
            eolConverter.Convert(data, dataLength, outputData, out outputLength);
        }
    }
}
