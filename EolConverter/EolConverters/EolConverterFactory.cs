using EolConverter.Encoding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EolConverter.EolConverters
{
    internal static class EolConverterFactory
    {
        internal static IEolConverter GetEolConverter(EolConversion conversion)
        {
            switch (conversion)
            {
                case EolConversion.CR:
                    {
                        return new CrEolConverter(new EncodingDetector());
                    }
                default:
                    {
                        return new NullEolConverter();
                    }
            }
        }
    }
}
