using EolConverter.ByteUtils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EolConverter.Encoding
{
    internal class EncodingDetectorFromEol
    {
        private Dictionary<EncodingType, Func<EncodingDetectionContext, bool>> encodingsToCheckOrdered;

        internal EncodingDetectorFromEol()
        {
            encodingsToCheckOrdered = new Dictionary<EncodingType, Func<EncodingDetectionContext, bool>>()
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
            var context = new EncodingDetectionContext(data, dataLength, eolIndexes);
            foreach (var encodingToCheck in encodingsToCheckOrdered)
            {
                var encoding = encodingToCheck.Key;
                var checkEncodingFunction = encodingToCheck.Value;
                if (checkEncodingFunction(context))
                {
                    return encoding;
                }
            }

            return EncodingType.None;
        }

        private bool IsUtf32LE(EncodingDetectionContext context)
        {
            return IsLittleEndianEncoding(context, EncodingType.Utf32LE);
        }

        private bool IsUtf32BE(EncodingDetectionContext context)
        {
            return IsBigEndianEncoding(context, EncodingType.Utf32BE);
        }

        private bool IsUtf16LE(EncodingDetectionContext context)
        {
            return IsLittleEndianEncoding(context, EncodingType.Utf16LE);
        }

        private bool IsUtf16BE(EncodingDetectionContext context)
        {
            return IsBigEndianEncoding(context, EncodingType.Utf16BE);
        }

        private bool IsUtf8(EncodingDetectionContext context)
        {
            // If there is any 0 within the data then can't be utf-8
            if (context.Data.Take(context.DataLength).Any(b => b == ByteCode.Null))
            {
                return false;
            }

            foreach (int eolIndex in context.EolIndexes)
            {
                // Check eol is not sorrounded by null bytes, checking also corner cases (eol is first or last bytes)
                if ((eolIndex == 0 || context.Data[eolIndex - 1] != ByteCode.Null)
                    && (eolIndex == context.DataLength - 1 || context.Data[eolIndex + 1] != ByteCode.Null))
                {
                    return true;
                }
            }

            return false;
        }

        private bool IsLittleEndianEncoding(EncodingDetectionContext context, EncodingType encoding)
        {
            int numBytesPerUnit = encoding.GetNumBytesPerUnit();
            // If there is a numBytesPerUnit of consecutive 0s within the data then can't be any of the encodings
            if (ContainsNullBytesUnit(context, numBytesPerUnit))
            {
                return false;
            }

            int numNullBytes = numBytesPerUnit - 1;
            foreach (int eolIndex in context.EolIndexes)
            {
                bool isFollowedByNullBytes = IsFollowedByNullBytes(context, eolIndex, numNullBytes);
                bool isFirstByteInUnit = eolIndex % numBytesPerUnit == 0;
                if (isFirstByteInUnit && isFollowedByNullBytes)
                {
                    return true;
                }
            }

            return false;
        }

        private bool IsBigEndianEncoding(EncodingDetectionContext context, EncodingType encoding)
        {
            int numBytesPerUnit = encoding.GetNumBytesPerUnit();
            // If there is a numBytesPerUnit of consecutive 0s within the data then can't be any of the encodings
            if (ContainsNullBytesUnit(context, numBytesPerUnit))
            {
                return false;
            }

            int numNullBytes = numBytesPerUnit - 1;
            foreach (int eolIndex in context.EolIndexes)
            {
                bool isPrecededByNullBytes = IsPrecededByNullBytes(context, eolIndex, numNullBytes);
                bool isLastByteInUnit = eolIndex % numBytesPerUnit == numBytesPerUnit - 1;
                if (isLastByteInUnit && isPrecededByNullBytes)
                {
                    return true;
                }
            }

            return false;
        }
        
        private bool ContainsNullBytesUnit(EncodingDetectionContext context, int numBytesPerUnit)
        {
            var nullBytesUnit = new byte[numBytesPerUnit];
            for (int i = 0; i <= context.DataLength - numBytesPerUnit; i += numBytesPerUnit)
            {
                if (context.Data.Skip(i).Take(numBytesPerUnit).All(b => b == ByteCode.Null))
                {
                    return true;
                }
            }

            return false;
        }

        private bool IsPrecededByNullBytes(EncodingDetectionContext context, int byteIndex, int numNullBytes)
        {
            if (byteIndex - numNullBytes < 0)
            {
                return false;
            }

            for (int i = 1; i <= numNullBytes; i++)
            {
                if (context.Data[byteIndex - i] != ByteCode.Null)
                {
                    return false;
                }
            }

            return true;
        }

        private bool IsFollowedByNullBytes(EncodingDetectionContext context, int byteIndex, int numNullBytes)
        {
            if (byteIndex + numNullBytes >= context.DataLength)
            {
                return false;
            }

            for (int i = numNullBytes; i > 0; i--)
            {
                if (context.Data[byteIndex + i] != ByteCode.Null)
                {
                    return false;
                }
            }

            return true;
        }

        private class EncodingDetectionContext
        {
            public byte[] Data { get; private set; }
            public int DataLength { get; private set; }
            public IEnumerable<int> EolIndexes { get; private set; }

            public EncodingDetectionContext(byte[] data, int dataLength, IEnumerable<int> eolIndexes)
            {
                Data = data;
                DataLength = dataLength;
                EolIndexes = eolIndexes;
            }
        }
    }
}
