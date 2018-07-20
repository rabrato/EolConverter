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
        Utf8,
        [NumBytesPerUnit(2)]
        Utf16LE,
        [NumBytesPerUnit(2)]
        Utf16BE,
        [NumBytesPerUnit(4)]
        Utf32LE,
        [NumBytesPerUnit(4)]
        Utf32BE
    }
}
