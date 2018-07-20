using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EolConverter.Encoding
{
    internal class EncodingDetectorFromBom
    {
        internal EncodingType GetEncodingFromBom(byte[] data, int dataLength)
        {
            if (HasUtf8Bom(data, dataLength))
            {
                return EncodingType.Utf8;
            }

            if (HasUtf16LeBom(data, dataLength))
            {
                return EncodingType.Utf16LE;
            }

            if (HasUtf16BeBom(data, dataLength))
            {
                return EncodingType.Utf16BE;
            }

            if (HasUtf32LeBom(data, dataLength))
            {
                return EncodingType.Utf32LE;
            }

            if (HasUtf32BeBom(data, dataLength))
            {
                return EncodingType.Utf32BE;
            }

            return EncodingType.None;
        }

        private bool HasUtf8Bom(byte[] data, int dataLength)
        {
            if (!HasMinLength(data, dataLength, minLength: 3))
            {
                return false;
            }

            return data[0] == 0xef && data[1] == 0xbb && data[2] == 0xbf;
        }

        private bool HasUtf16LeBom(byte[] data, int dataLength)
        {
            if (!HasMinLength(data, dataLength, minLength: 2))
            {
                return false;
            }

            return data[0] == 0xff && data[1] == 0xfe && !HasUtf32LeBom(data, dataLength);
        }

        private bool HasUtf16BeBom(byte[] data, int dataLength)
        {
            if (!HasMinLength(data, dataLength, minLength: 2))
            {
                return false;
            }

            return data[0] == 0xfe && data[1] == 0xff;
        }

        private bool HasUtf32LeBom(byte[] data, int dataLength)
        {
            if (!HasMinLength(data, dataLength, minLength: 4))
            {
                return false;
            }

            return data[0] == 0xff && data[1] == 0xfe && data[2] == 0 && data[3] == 0;
        }

        private bool HasUtf32BeBom(byte[] data, int dataLength)
        {
            if (!HasMinLength(data, dataLength, minLength: 4))
            {
                return false;
            }

            return data[0] == 0 && data[1] == 0 && data[2] == 0xfe && data[3] == 0xff;
        }

        private bool HasMinLength(byte[] data, int dataLength, int minLength)
        {
            return data.Length >= minLength && dataLength >= minLength;
        }
    }
}
