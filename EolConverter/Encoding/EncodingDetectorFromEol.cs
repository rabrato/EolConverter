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

        /// <summary>
        /// Returns the encoding of the data if can be detected through any of the end of line bytes.
        /// </summary>
        /// <param name="data">Data where to detect encoding.</param>
        /// <param name="dataLength">Length of the data.</param>
        /// <returns>The encoding if detected.</returns>
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

            return context.EolIndexes.Any(eolIndex => {
                // Check eol is not sorrounded by null bytes, checking also corner cases (eol is first or last bytes)
                bool notPrecededByNull = (eolIndex == 0 || context.Data[eolIndex - 1] != ByteCode.Null);
                bool notFollowedByNull = (eolIndex == context.DataLength - 1 || context.Data[eolIndex + 1] != ByteCode.Null);
                return notPrecededByNull && notFollowedByNull;
            });
        }

        private bool IsLittleEndianEncoding(EncodingDetectionContext context, EncodingType encoding)
        {
            return IsFromEncoding(context, encoding, IsLittleEndian);
        }

        private bool IsBigEndianEncoding(EncodingDetectionContext context, EncodingType encoding)
        {
            return IsFromEncoding(context, encoding, IsBigEndian);
        }

        private bool IsFromEncoding(
            EncodingDetectionContext context, 
            EncodingType encoding, 
            Func<EncodingDetectionContext, int, bool> IsFromEndiannes)
        {
            int numBytesPerUnit = encoding.GetNumBytesPerUnit();
            // If there is a entire null unit within the data then can't be this encoding
            if (ContainsNullBytesUnit(context.Data, context.DataLength, numBytesPerUnit))
            {
                return false;
            }

            return IsFromEndiannes(context, numBytesPerUnit);
        }

        private bool IsLittleEndian(EncodingDetectionContext context, int numBytesPerUnit)
        {
            return context.EolIndexes.Any(eolIndex => {
                bool isFollowedByNullBytes = IsFollowedByNullBytes(context.Data, context.DataLength, eolIndex, numNullBytes: numBytesPerUnit - 1);
                bool isFirstByteInUnit = eolIndex % numBytesPerUnit == 0;
                return isFirstByteInUnit && isFollowedByNullBytes;
            });
        }

        private bool IsBigEndian(EncodingDetectionContext context, int numBytesPerUnit)
        {
            return context.EolIndexes.Any(eolIndex => {
                bool isPrecededByNullBytes = IsPrecededByNullBytes(context.Data, eolIndex, numNullBytes: numBytesPerUnit - 1);
                bool isLastByteInUnit = eolIndex % numBytesPerUnit == numBytesPerUnit - 1;
                return isLastByteInUnit && isPrecededByNullBytes;
            });
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
