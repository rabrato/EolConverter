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

        public void Convert(byte[] data, int dataLength, byte[] outputData, out int outputLength)
        {
            eolConverter.Convert(data, dataLength, outputData, out outputLength);
        }
    }
}
