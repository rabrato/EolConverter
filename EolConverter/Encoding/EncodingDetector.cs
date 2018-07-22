namespace EolConverter.Encoding
{
    public class EncodingDetector : IEncodingDetector
    {
        private EncodingDetectorFromBom detectorFromBom;
        private EncodingDetectorFromEol detectorFromEol;

        public EncodingDetector()
        {
            detectorFromBom = new EncodingDetectorFromBom();
            detectorFromEol = new EncodingDetectorFromEol();
        }

        /// <summary>
        /// Returns the encoding of the data if detected, also returns if the data has the byte order mark.
        /// </summary>
        /// <param name="data">Data where to detect encoding.</param>
        /// <param name="dataLength">Length of the data.</param>
        /// <returns>Returns a tuple with the encoding if detected, and the byte order mark information.</returns>
        public (EncodingType encoding, bool hasBom) GetEncoding(byte[] data, int dataLength)
        {
            if (data == null || data.Length == 0 || dataLength == 0)
            {
                return (EncodingType.None, hasBom: false);
            }

            var encodingFromBom = detectorFromBom.GetEncodingFromBom(data, dataLength);
            if (encodingFromBom != EncodingType.None)
            {
                return (encodingFromBom, hasBom: true);
            }

            var encodingFromEol = detectorFromEol.GetEncodingFromEolBytes(data, dataLength);
            return (encodingFromEol, hasBom: false);
        }
    }
}
