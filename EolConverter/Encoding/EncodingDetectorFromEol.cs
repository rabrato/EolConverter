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

            if (IstUtf8Encoding(data, eolIndex.Value, dataLength))
            {
                return EncodingType.Utf8;
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

            // If there is a numBytesPerUnit of consecutive 0s within the data then can't be any of the encodings
            if (ContainsZeroBytesUnit(data, dataLength, numBytesPerUnit))
            {
                return EncodingType.None;
            }

            int numZeros = numBytesPerUnit - 1;

            bool isPrecededByZeros = IsPrecededByZeros(data, eolIndex, numZeros);
            bool isFollowedByZeros = IsFollowedByZeros(data, eolIndex, numZeros, dataLength);

            bool isLastByteInUnit = eolIndex % numBytesPerUnit == numBytesPerUnit - 1;
            bool isFirstByteInUnit = eolIndex % numBytesPerUnit == 0;

            if (isFirstByteInUnit && isFollowedByZeros)
            {
                return leEncoding;
            }

            if (isLastByteInUnit && isPrecededByZeros)
            {
                return beEncoding;
            }

            return EncodingType.None;
        }

        private bool ContainsZeroBytesUnit(byte[] data, int dataLength, int numBytesPerUnit)
        {
            var zeroBytesUnit = new byte[numBytesPerUnit];
            for (int i = 0; i < dataLength - numBytesPerUnit; i++)
            {
                if (data.Skip(i).Take(numBytesPerUnit).All(b => b == EolByte.Zero))
                {
                    return true;
                }
            }

            return false;
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

        private bool IstUtf8Encoding(byte[] data, int eolIndex, int dataLength)
        {
            // If there is any 0 within the data then can't be utf-8
            if (data.Take(dataLength).Any(b => b == EolByte.Zero))
            {
                return false;
            }

            // Check eol is not sorrounded by zero bytes, checking also corner cases (eol is first or last bytes)
            if ((eolIndex == 0 || data[eolIndex - 1] != EolByte.Zero)
                && (eolIndex == dataLength - 1 || data[eolIndex + 1] != EolByte.Zero))
            {
                return true;
            }

            return false;
        }
    }
}
