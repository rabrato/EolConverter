using EolConverter.ByteUtils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EolConverter.Encoding
{
    internal class EncodingDetectorFromEol
    {
        private Dictionary<EncodingType, Func<byte[], int, IEnumerable<int>, bool>> encodingsToCheckOrdered;

        public EncodingDetectorFromEol()
        {
            encodingsToCheckOrdered = new Dictionary<EncodingType, Func<byte[], int, IEnumerable<int>, bool>>()
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
            var eolIndexes = data.FindEolByteIndexes();
            if (!eolIndexes.Any())
            {
                // If there are not eol bytes then the encoding is not needed because the file will not need eol replacement.
                return EncodingType.None;
            }

            foreach (var encodingToCheck in encodingsToCheckOrdered)
            {
                var encoding = encodingToCheck.Key;
                var checkEncodingFunction = encodingToCheck.Value;
                if (checkEncodingFunction(data, dataLength, eolIndexes))
                {
                    return encoding;
                }
            }

            return EncodingType.None;
        }

        private bool IsUtf32LE(byte[] data, int dataLength, IEnumerable<int> eolIndexes)
        {
            return IsLittleEndianEncoding(data, dataLength, eolIndexes, EncodingType.Utf32LE);
        }

        private bool IsUtf32BE(byte[] data, int dataLength, IEnumerable<int> eolIndexes)
        {
            return IsBigEndianEncoding(data, dataLength, eolIndexes, EncodingType.Utf32BE);
        }

        private bool IsUtf16LE(byte[] data, int dataLength, IEnumerable<int> eolIndexes)
        {
            return IsLittleEndianEncoding(data, dataLength, eolIndexes, EncodingType.Utf16LE);
        }

        private bool IsUtf16BE(byte[] data, int dataLength, IEnumerable<int> eolIndexes)
        {
            return IsBigEndianEncoding(data, dataLength, eolIndexes, EncodingType.Utf16BE);
        }

        private bool IsUtf8(byte[] data, int dataLength, IEnumerable<int> eolIndexes)
        {
            // If there is any 0 within the data then can't be utf-8
            if (data.Take(dataLength).Any(b => b == ByteCode.Null))
            {
                return false;
            }

            foreach (int eolIndex in eolIndexes)
            {
                // Check eol is not sorrounded by null bytes, checking also corner cases (eol is first or last bytes)
                if ((eolIndex == 0 || data[eolIndex - 1] != ByteCode.Null)
                    && (eolIndex == dataLength - 1 || data[eolIndex + 1] != ByteCode.Null))
                {
                    return true;
                }
            }

            return false;
        }

        private bool IsLittleEndianEncoding(byte[] data, int dataLength, IEnumerable<int> eolIndexes, EncodingType encoding)
        {
            int numBytesPerUnit = encoding.GetNumBytesPerUnit();
            // If there is a numBytesPerUnit of consecutive 0s within the data then can't be any of the encodings
            if (ContainsNullBytesUnit(data, dataLength, numBytesPerUnit))
            {
                return false;
            }

            int numNullBytes = numBytesPerUnit - 1;
            foreach (int eolIndex in eolIndexes)
            {
                bool isFollowedByNullBytes = IsFollowedByNullBytes(data, dataLength, eolIndex, numNullBytes);
                bool isFirstByteInUnit = eolIndex % numBytesPerUnit == 0;
                if (isFirstByteInUnit && isFollowedByNullBytes)
                {
                    return true;
                }
            }

            return false;
        }

        private bool IsBigEndianEncoding(byte[] data, int dataLength, IEnumerable<int> eolIndexes, EncodingType encoding)
        {
            int numBytesPerUnit = encoding.GetNumBytesPerUnit();
            // If there is a numBytesPerUnit of consecutive 0s within the data then can't be any of the encodings
            if (ContainsNullBytesUnit(data, dataLength, numBytesPerUnit))
            {
                return false;
            }

            int numNullBytes = numBytesPerUnit - 1;
            foreach (int eolIndex in eolIndexes)
            {
                bool isPrecededByNullBytes = IsPrecededByNullBytes(data, eolIndex, numNullBytes);
                bool isLastByteInUnit = eolIndex % numBytesPerUnit == numBytesPerUnit - 1;
                if (isLastByteInUnit && isPrecededByNullBytes)
                {
                    return true;
                }
            }

            return false;
        }
        
        private bool ContainsNullBytesUnit(byte[] data, int dataLength, int numBytesPerUnit)
        {
            var nullBytesUnit = new byte[numBytesPerUnit];
            for (int i = 0; i <= dataLength - numBytesPerUnit; i += numBytesPerUnit)
            {
                if (data.Skip(i).Take(numBytesPerUnit).All(b => b == ByteCode.Null))
                {
                    return true;
                }
            }

            return false;
        }

        private bool IsPrecededByNullBytes(byte[] data, int byteIndex, int numNullBytes)
        {
            if (byteIndex - numNullBytes < 0)
            {
                return false;
            }

            for (int i = 1; i <= numNullBytes; i++)
            {
                if (data[byteIndex - i] != ByteCode.Null)
                {
                    return false;
                }
            }

            return true;
        }

        private bool IsFollowedByNullBytes(byte[] data, int dataLength, int byteIndex, int numNullBytes)
        {
            if (byteIndex + numNullBytes >= dataLength)
            {
                return false;
            }

            for (int i = numNullBytes; i > 0; i--)
            {
                if (data[byteIndex + i] != ByteCode.Null)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
