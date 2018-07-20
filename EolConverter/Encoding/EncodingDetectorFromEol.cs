using EolConverter.EolConverters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EolConverter.Encoding
{
    public static class EncodingDetectorFromEol
    {
        public static EncodingType GetEncodingFromEolBytes(this byte[] data, int dataLength)
        {
            int? eolIndex = data.FindFirstEnfOfLineByteIndex();

            if (eolIndex == null)
            {
                // If there are not eol bytes then the encoding is not needed because the file will not need eol replacement.
                return EncodingType.None;
            }

            var utf32Encoding = data.GetUtf32Encoding(eolIndex.Value, dataLength);
            if (utf32Encoding != EncodingType.None)
            {
                return utf32Encoding;
            }

            var utf16Encoding = data.GetUtf16Encoding(eolIndex.Value, dataLength);
            if (utf16Encoding != EncodingType.None)
            {
                return utf16Encoding;
            }

            var utf8Encoding = data.GetUtf8Encoding(eolIndex.Value, dataLength);
            if (utf8Encoding != EncodingType.None)
            {
                return utf8Encoding;
            }

            return EncodingType.None;
        }

        private static EncodingType GetUtf32Encoding(this byte[] data, int eolIndex, int dataLength)
        {
            return data.GetMultipleByteEncoding(eolIndex, dataLength, EncodingType.Utf32LE, EncodingType.Utf32BE);
        }

        private static EncodingType GetUtf16Encoding(this byte[] data, int eolIndex, int dataLength)
        {
            return data.GetMultipleByteEncoding(eolIndex, dataLength, EncodingType.Utf16LE, EncodingType.Utf16BE);
        }

        private static EncodingType GetUtf8Encoding(this byte[] data, int eolIndex, int dataLength)
        {
            // Check if the first EOL found is CRLF
            if (data[eolIndex] == EolByte.Cr && data[eolIndex + 1] == EolByte.Lf)
            {
                return data.GetUtf8EncodingWhenFirstEolIsCrLf(eolIndex, dataLength);
            }
            else
            {
                return data.GetUtf8EncodingWhenFirstEolIsSimple(eolIndex, dataLength);
            }
        }

        private static EncodingType GetUtf8EncodingWhenFirstEolIsCrLf(this byte[] data, int crIndex, int dataLength)
        {
            int lfIndex = crIndex + 1;
            // If CRLF are the first bytes and are not followed by a zero
            if (crIndex == 0 && data[lfIndex + 1] != EolByte.Zero)
            {
                return EncodingType.Utf8;
            }

            // If CRLF are the last bytes and are not preceded by a zero
            if (crIndex == dataLength - 2 && data[crIndex - 1] != EolByte.Zero)
            {
                return EncodingType.Utf8;
            }

            // If CRLF are sorrounded by not zero bytes
            if (data[crIndex - 1] != EolByte.Zero && data[lfIndex + 1] != EolByte.Zero)
            {
                return EncodingType.Utf8;
            }

            return EncodingType.None;
        }

        private static EncodingType GetUtf8EncodingWhenFirstEolIsSimple(this byte[] data, int eolIndex, int dataLength)
        {
            // If EOL is the first byte and is not followed by a zero
            if (eolIndex == 0 && data[1] != EolByte.Zero)
            {
                return EncodingType.Utf8;
            }


            // If EOL is the last byte and is not preceded by a zero
            if (eolIndex == dataLength - 1 && data[dataLength - 2] != EolByte.Zero)
            {
                return EncodingType.Utf8;
            }

            // If EOL IS sorrounded by not zero bytes
            if (data[eolIndex - 1] != EolByte.Zero && data[eolIndex + 1] != EolByte.Zero)
            {
                return EncodingType.Utf8;
            }

            return EncodingType.None;
        }

        private static EncodingType GetMultipleByteEncoding(this byte[] data, int eolIndex,
            int dataLength, EncodingType leEncoding, EncodingType beEncoding)
        {
            int numBytesPerUnit = leEncoding.GetNumBytesPerUnit();
            int numZeros = numBytesPerUnit - 1;

            bool isPrecededByZeros = data.IsPrecededByZeros(eolIndex, numZeros);
            bool isFollowedByZeros = data.IsFollowedByZeros(eolIndex, numZeros, dataLength);
            if (isPrecededByZeros && isFollowedByZeros)
            {
                // Check if the first EOL found is CRLF
                int crIndex = eolIndex;
                int lfIndex = eolIndex + numBytesPerUnit;
                if (data[crIndex] == EolByte.Cr && data[lfIndex] == EolByte.Lf)
                {
                    bool isCrLfPrecededByZeros = isPrecededByZeros;
                    bool isCrLfFollowedByZeros = data.IsFollowedByZeros(lfIndex, numZeros, dataLength);
                    return GetEncodingFromSurroundingZeros(isCrLfPrecededByZeros, isCrLfFollowedByZeros, leEncoding, beEncoding);
                }
            }
            else
            {
                return GetEncodingFromSurroundingZeros(isPrecededByZeros, isFollowedByZeros, leEncoding, beEncoding);
            }

            return EncodingType.None;
        }

        private static EncodingType GetEncodingFromSurroundingZeros(bool isPrecededByZeros, bool isFollowedByZeros,
            EncodingType leEncoding, EncodingType beEncoding)
        {
            if (isPrecededByZeros && isFollowedByZeros)
            {
                return EncodingType.None;
            }

            if (isPrecededByZeros)
            {
                return beEncoding;
            }

            if (isFollowedByZeros)
            {
                return leEncoding;
            }

            return EncodingType.None;
        }

        private static bool IsPrecededByZeros(this byte[] data, int byteIndex, int numZeros)
        {
            if (byteIndex - numZeros < 0)
            {
                return false;
            }

            for (int i = 0; i < numZeros; i++)
            {
                if (data[byteIndex - i] != EolByte.Zero)
                {
                    return false;
                }
            }

            return true;
        }

        private static bool IsFollowedByZeros(this byte[] data, int byteIndex, int numZeros, int dataLength)
        {
            if (byteIndex + numZeros >= dataLength)
            {
                return false;
            }

            for (int i = numZeros; i > 0; i--)
            {
                if (data[byteIndex + i] != EolByte.Zero)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
