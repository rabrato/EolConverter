using EolConverter.ByteUtils;
using EolConverter.ComponentModel;
using EolConverter.Encoding;

namespace EolConverter
{
    public static class EolConversionExtensions
    {
        /// <summary>
        /// Returns the end of line units of the conversion, using the specified encoding.
        /// </summary>
        /// <param name="eolConversion">The conversion to get end of line units from.</param>
        /// <param name="encoding">The encoding used to create the units.</param>
        /// <returns>An array with the end of line units of the conversion.</returns>
        public static byte[] GetEolUnits(this EolConversion eolConversion, EncodingType encoding)
        {
            return eolConversion
                .GetEolBytes()
                .ToUnits(encoding);
        }

        private static byte[] GetEolBytes(this EolConversion eolConversion)
        {
            var attribute = eolConversion.GetAttribute<EolBytesAttribute>();
            return attribute?.EolBytes ?? new byte[0];
        }
    }
}
