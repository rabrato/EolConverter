using EolConverter;
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
        [ByteOrderMark(new byte[3] { 0xef, 0xbb, 0xbf })]
        Utf8,

        [NumBytesPerUnit(2)]
        [ByteOrderMark(new byte[2] { 0xff, 0xfe })]
        Utf16LE,

        [NumBytesPerUnit(2)]
        [ByteOrderMark(new byte[2] { 0xfe, 0xff })]
        Utf16BE,

        [NumBytesPerUnit(4)]
        [ByteOrderMark(new byte[4] { 0xff, 0xfe, ByteCode.Empty, ByteCode.Empty })]
        Utf32LE,

        [NumBytesPerUnit(4)]
        [ByteOrderMark(new byte[4] { ByteCode.Empty, ByteCode.Empty, 0xfe, 0xff })]
        Utf32BE
    }
}
