namespace EolConverter.Encoding
{
    public interface IEncodingDetector
    {
        (EncodingType encoding, bool hasBom) GetEncoding(byte[] data, int dataLength);
    }
}
