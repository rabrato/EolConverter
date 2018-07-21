using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EolConverter.Encoding
{
    public static class EncodingTypeExtensions
    {
        public static bool IsBigEndian(this EncodingType encoding)
        {
            return encoding == EncodingType.Utf16BE || encoding == EncodingType.Utf32BE;
        }

        public static int GetNumBytesPerUnit(this EncodingType encoding)
        {
            var attribute = encoding.GetAttribute<NumBytesPerUnitAttribute>();
            return attribute?.NumBytesPerUnit ?? 0;
        }

        public static byte[] GetByteOrderMark(this EncodingType encoding)
        {
            var attribute = encoding.GetAttribute<ByteOrderMarkAttribute>();
            return attribute?.ByteOrderMark ?? new byte[0];
        }

        private static T GetAttribute<T>(this EncodingType encoding)
            where T : Attribute
        {
            var type = encoding.GetType();
            var memInfo = type.GetMember(encoding.ToString());
            var attributes = memInfo[0].GetCustomAttributes(typeof(T), false);
            return ((T)attributes[0]);
        }
    }
}
