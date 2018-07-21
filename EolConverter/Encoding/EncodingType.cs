using EolConverter.ByteUtils;
using EolConverter.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EolConverter.Encoding
{
    public enum EncodingType
    {
        None,

        [NumBytesPerUnit(1)]
        [ByteOrderMark(0xef, 0xbb, 0xbf)]
        Utf8,

        [NumBytesPerUnit(2)]
        [ByteOrderMark(0xff, 0xfe)]
        Utf16LE,

        [NumBytesPerUnit(2)]
        [ByteOrderMark(0xfe, 0xff)]
        Utf16BE,

        [NumBytesPerUnit(4)]
        [ByteOrderMark(0xff, 0xfe, ByteCode.Empty, ByteCode.Empty)]
        Utf32LE,

        [NumBytesPerUnit(4)]
        [ByteOrderMark(ByteCode.Empty, ByteCode.Empty, 0xfe, 0xff)]
        Utf32BE
    }
}
