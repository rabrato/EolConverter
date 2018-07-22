using EolConverter.ComponentModel;

namespace EolConverter.Encoding
{
    public static class EncodingTypeExtensions
    {
        /// <summary>
        /// Return wheter the encoding as a big endianness or not.
        /// </summary>
        /// <param name="encoding">The encoding to be checked.</param>
        /// <returns>True if the encoding is big endian, false otherwise.</returns>
        public static bool IsBigEndian(this EncodingType encoding)
        {
            return encoding == EncodingType.Utf16BE || encoding == EncodingType.Utf32BE;
        }

        /// <summary>
        /// Return the number of byters per unit for the encoding.
        /// </summary>
        /// <param name="encoding">The encoding to be checked.</param>
        /// <returns>The number of bytes per unit</returns>
        public static int GetNumBytesPerUnit(this EncodingType encoding)
        {
            var attribute = encoding.GetAttribute<NumBytesPerUnitAttribute>();
            return attribute?.NumBytesPerUnit ?? 0;
        }

        /// <summary>
        /// Return the encoding byte order mark.
        /// </summary>
        /// <param name="encoding">The encoding to be checked.</param>
        /// <returns>The encoding byte order mark.</returns>
        public static byte[] GetByteOrderMark(this EncodingType encoding)
        {
            var attribute = encoding.GetAttribute<ByteOrderMarkAttribute>();
            return attribute?.ByteOrderMark ?? new byte[0];
        }
    }
}
