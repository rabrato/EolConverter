using EolConverter.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EolConverter
{
    public static class EolConversionExtensions
    {
        public static byte[] GetEolBytes(this EolConversion eolConversion)
        {
            var attribute = eolConversion.GetAttribute<EolBytesAttribute>();
            return attribute?.EolBytes ?? new byte[0];
        }
    }
}
