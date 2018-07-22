using EolConverter.ByteUtils;
using EolConverter.ComponentModel;
using EolConverter.Encoding;

namespace EolConverter
{
    public static class EolConversionExtensions
    {
        public static byte[] GetEolBytes(this EolConversion eolConversion)
        {
            var attribute = eolConversion.GetAttribute<EolBytesAttribute>();
            return attribute?.EolBytes ?? new byte[0];
        }

        public static byte[] GetEolUnits(this EolConversion eolConversion, EncodingType encoding)
        {
            return eolConversion
                .GetEolBytes()
                .ToUnits(encoding);
        }
    }
}
