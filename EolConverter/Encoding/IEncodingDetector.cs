namespace EolConverter.Encoding
{
    public interface IEncodingDetector
    {
        /// <summary>
        /// Returns the encoding of the data if detected, also returns if the data has the byte order mark.
        /// </summary>
        /// <param name="data">Data where to detect encoding.</param>
        /// <param name="dataLength">Length of the data.</param>
        /// <returns>Returns a tuple with the encoding if detected, and the byte order mark information.</returns>
        (EncodingType encoding, bool hasBom) GetEncoding(byte[] data, int dataLength);
    }
}
