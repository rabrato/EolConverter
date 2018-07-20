using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EolConverter.Encoding
{
    public static class EncodingDetectorFromBom
    {
        public static EncodingType GetEncodingFromBom(this byte[] data)
        {
            if (data.HasUtf8Bom())
            {
                return EncodingType.Utf8;
            }

            if (data.HasUtf16LeBom())
            {
                return EncodingType.Utf16LE;
            }

            if (data.HasUtf16BeBom())
            {
                return EncodingType.Utf16BE;
            }

            if (data.HasUtf32LeBom())
            {
                return EncodingType.Utf32LE;
            }

            if (data.HasUtf32BeBom())
            {
                return EncodingType.Utf32BE;
            }

            return EncodingType.None;
        }

        private static bool HasUtf8Bom(this byte[] data)
        {
            if (data.Length < 3)
            {
                return false;
            }

            return data[0] == 0xef && data[1] == 0xbb && data[2] == 0xbf;
        }

        private static bool HasUtf16LeBom(this byte[] data)
        {
            if (data.Length < 2)
            {
                return false;
            }

            return data[0] == 0xff && data[1] == 0xfe && !data.HasUtf32LeBom();
        }

        private static bool HasUtf16BeBom(this byte[] data)
        {
            if (data.Length < 2)
            {
                return false;
            }

            return data[0] == 0xfe && data[1] == 0xff;
        }

        private static bool HasUtf32LeBom(this byte[] data)
        {
            if (data.Length < 4)
            {
                return false;
            }

            return data[0] == 0xff && data[1] == 0xfe && data[2] == 0 && data[3] == 0;
        }

        private static bool HasUtf32BeBom(this byte[] data)
        {
            if (data.Length < 4)
            {
                return false;
            }

            return data[0] == 0 && data[1] == 0 && data[2] == 0xfe && data[3] == 0xff;
        }
    }
}
