using EolConverter.EolConverters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EolConverter.Encoding
{
    internal class EncodingDetectorFromEol
    {
        internal EncodingType GetEncodingFromEolBytes(byte[] data, int dataLength)
        {
            int? eolIndex = data.FindFirstEnfOfLineByteIndex();

            if (eolIndex == null)
            {
                // If there are not eol bytes then the encoding is not needed because the file will not need eol replacement.
                return EncodingType.None;
            }

            var utf32Encoding = GetUtf32Encoding(data, eolIndex.Value, dataLength);
            if (utf32Encoding != EncodingType.None)
            {
                return utf32Encoding;
            }

            var utf16Encoding = GetUtf16Encoding(data, eolIndex.Value, dataLength);
            if (utf16Encoding != EncodingType.None)
            {
                return utf16Encoding;
            }

            var utf8Encoding = GetUtf8Encoding(data, eolIndex.Value, dataLength);
            if (utf8Encoding != EncodingType.None)
            {
                return utf8Encoding;
            }

            return EncodingType.None;
        }

        private EncodingType GetUtf32Encoding(byte[] data, int eolIndex, int dataLength)
        {
            return GetMultipleByteEncoding(data, eolIndex, dataLength, EncodingType.Utf32LE, EncodingType.Utf32BE);
        }

        private EncodingType GetUtf16Encoding(byte[] data, int eolIndex, int dataLength)
        {
            return GetMultipleByteEncoding(data, eolIndex, dataLength, EncodingType.Utf16LE, EncodingType.Utf16BE);
        }

        private EncodingType GetMultipleByteEncoding(byte[] data, int eolIndex,
            int dataLength, EncodingType leEncoding, EncodingType beEncoding)
        {
            int numBytesPerUnit = leEncoding.GetNumBytesPerUnit();
            int numZeros = numBytesPerUnit - 1;

            bool isPrecededByZeros = IsPrecededByZeros(data, eolIndex, numZeros);
            bool isFollowedByZeros = IsFollowedByZeros(data, eolIndex, numZeros, dataLength);
            if (isPrecededByZeros && isFollowedByZeros)
            {
                // Check if the first EOL found is CRLF
                int crIndex = eolIndex;
                int lfIndex = eolIndex + numBytesPerUnit;
                if (data[crIndex] == EolByte.Cr && data[lfIndex] == EolByte.Lf)
                {
                    bool isCrLfPrecededByZeros = isPrecededByZeros;
                    bool isCrLfFollowedByZeros = IsFollowedByZeros(data, lfIndex, numZeros, dataLength);
                    return GetEncodingFromSurroundingZeros(isCrLfPrecededByZeros, isCrLfFollowedByZeros, leEncoding, beEncoding);
                }
            }
            else
            {
                return GetEncodingFromSurroundingZeros(isPrecededByZeros, isFollowedByZeros, leEncoding, beEncoding);
            }

            return EncodingType.None;
        }

        private EncodingType GetEncodingFromSurroundingZeros(bool isPrecededByZeros, bool isFollowedByZeros,
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

        private bool IsPrecededByZeros(byte[] data, int byteIndex, int numZeros)
        {
            if (byteIndex - numZeros < 0)
            {
                return false;
            }

            for (int i = 1; i <= numZeros; i++)
            {
                if (data[byteIndex - i] != EolByte.Zero)
                {
                    return false;
                }
            }

            return true;
        }

        private bool IsFollowedByZeros(byte[] data, int byteIndex, int numZeros, int dataLength)
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

        private EncodingType GetUtf8Encoding(byte[] data, int eolIndex, int dataLength)
        {
            int byteIndexPreviousToEol;
            int byteIndexNextToEol;
            // Check if the first EOL found is CRLF
            if (data[eolIndex] == EolByte.Cr && eolIndex + 1 < dataLength && data[eolIndex + 1] == EolByte.Lf)
            {
                int crIndex = eolIndex;
                int lfIndex = eolIndex + 1;
                byteIndexPreviousToEol = crIndex - 1;
                byteIndexNextToEol = lfIndex + 1;
            }
            else
            {
                byteIndexPreviousToEol = eolIndex - 1;
                byteIndexNextToEol = eolIndex + 1;
            }

            // Check eol is not sorrounded by zero bytes, checking also corner cases (eol is first or last bytes)
            if ((byteIndexPreviousToEol < 0 || data[byteIndexPreviousToEol] != EolByte.Zero)
                && (byteIndexNextToEol >= dataLength || data[byteIndexNextToEol + 1] != EolByte.Zero))
            {
                return EncodingType.Utf8;
            }

            return EncodingType.None;
        }
    }
}
