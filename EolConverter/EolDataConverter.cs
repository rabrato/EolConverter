using EolConverter.Encoding;

namespace EolConverter
{
    public class EolDataConverter
    {
        private readonly EolConverter eolConverter;

        public EolDataConverter(EolConversion eolConversion)
        {
            eolConverter = new EolConverter(eolConversion, new EncodingDetector());
        }

        /// <summary>
        /// Fills <paramref name="outputData"/> with the data after applying the end of line bytes conversion.
        /// </summary>
        /// <param name="data">The data to convert.</param>
        /// <param name="dataLength">The length of the data to convert.</param>
        /// <param name="outputData">The array where to store the converted data.</param>
        /// <param name="outputLength">The length of the converted data.</param>
        public void Convert(byte[] data, int dataLength, byte[] outputData, out int outputLength)
        {
            eolConverter.Convert(data, dataLength, outputData, out outputLength);
        }
    }
}
