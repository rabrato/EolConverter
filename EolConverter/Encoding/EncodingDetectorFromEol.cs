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
        private Dictionary<EncodingType, Func<byte[], int, int, bool>> encodingsToCheckOrdered;

        public EncodingDetectorFromEol()
        {
            encodingsToCheckOrdered = new Dictionary<EncodingType, Func<byte[], int, int, bool>>()
            {
                [EncodingType.Utf32BE] = IsUtf32BE,
                [EncodingType.Utf32LE] = IsUtf32LE,
                [EncodingType.Utf16BE] = IsUtf16BE,
                [EncodingType.Utf16LE] = IsUtf16LE,
                [EncodingType.Utf8] = IsUtf8,
            };
        }

        internal EncodingType GetEncodingFromEolBytes(byte[] data, int dataLength)
        {
            int? eolIndex = data.FindFirstEnfOfLineByteIndex();
            if (eolIndex == null)
            {
                // If there are not eol bytes then the encoding is not needed because the file will not need eol replacement.
                return EncodingType.None;
            }

            foreach (var encodingToCheck in encodingsToCheckOrdered)
            {
                var encoding = encodingToCheck.Key;
                var checkEncodingFunction = encodingToCheck.Value;
                if (checkEncodingFunction(data, dataLength, eolIndex.Value))
                {
                    return encoding;
                }
            }

            return EncodingType.None;
        }

        private bool IsUtf32LE(byte[] data, int dataLength, int eolIndex)
        {
            return IsLittleEndianMultipleByteEncoding(data, dataLength, eolIndex, EncodingType.Utf32LE);
        }

        private bool IsUtf32BE(byte[] data, int dataLength, int eolIndex)
        {
            return IsBigEndianMultipleByteEncoding(data, dataLength, eolIndex, EncodingType.Utf32BE);
        }

        private bool IsUtf16LE(byte[] data, int dataLength, int eolIndex)
        {
            return IsLittleEndianMultipleByteEncoding(data, dataLength, eolIndex, EncodingType.Utf16LE);
        }

        private bool IsUtf16BE(byte[] data, int dataLength, int eolIndex)
        {
            return IsBigEndianMultipleByteEncoding(data, dataLength, eolIndex, EncodingType.Utf16BE);
        }

        private bool IsUtf8(byte[] data, int dataLength, int eolIndex)
        {
            // If there is any 0 within the data then can't be utf-8
            if (data.Take(dataLength).Any(b => b == EolByte.Empty))
            {
                return false;
            }

            // Check eol is not sorrounded by zero bytes, checking also corner cases (eol is first or last bytes)
            if ((eolIndex == 0 || data[eolIndex - 1] != EolByte.Empty)
                && (eolIndex == dataLength - 1 || data[eolIndex + 1] != EolByte.Empty))
            {
                return true;
            }

            return false;
        }

        private bool IsLittleEndianMultipleByteEncoding(byte[] data, int dataLength, int eolIndex, EncodingType encoding)
        {
            int numBytesPerUnit = encoding.GetNumBytesPerUnit();
            // If there is a numBytesPerUnit of consecutive 0s within the data then can't be any of the encodings
            if (ContainsZeroBytesUnit(data, dataLength, numBytesPerUnit))
            {
                return false;
            }

            int numZeros = numBytesPerUnit - 1;
            bool isFollowedByZeros = IsFollowedByZeros(data, dataLength, eolIndex, numZeros);
            bool isFirstByteInUnit = eolIndex % numBytesPerUnit == 0;
            return isFirstByteInUnit && isFollowedByZeros;
        }

        private bool IsBigEndianMultipleByteEncoding(byte[] data, int dataLength, int eolIndex, EncodingType encoding)
        {
            int numBytesPerUnit = encoding.GetNumBytesPerUnit();
            // If there is a numBytesPerUnit of consecutive 0s within the data then can't be any of the encodings
            if (ContainsZeroBytesUnit(data, dataLength, numBytesPerUnit))
            {
                return false;
            }

            int numZeros = numBytesPerUnit - 1;
            bool isPrecededByZeros = IsPrecededByZeros(data, eolIndex, numZeros);
            bool isLastByteInUnit = eolIndex % numBytesPerUnit == numBytesPerUnit - 1;
            return isLastByteInUnit && isPrecededByZeros;
        }
        
        private bool ContainsZeroBytesUnit(byte[] data, int dataLength, int numBytesPerUnit)
        {
            var zeroBytesUnit = new byte[numBytesPerUnit];
            for (int i = 0; i < dataLength - numBytesPerUnit; i++)
            {
                if (data.Skip(i).Take(numBytesPerUnit).All(b => b == EolByte.Empty))
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
                if (data[byteIndex - i] != EolByte.Empty)
                {
                    return false;
                }
            }

            return true;
        }

        private bool IsFollowedByZeros(byte[] data, int dataLength, int byteIndex, int numZeros)
        {
            if (byteIndex + numZeros >= dataLength)
            {
                return false;
            }

            for (int i = numZeros; i > 0; i--)
            {
                if (data[byteIndex + i] != EolByte.Empty)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
