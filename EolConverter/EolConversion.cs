using EolConverter.ByteUtils;
using EolConverter.ComponentModel;

namespace EolConverter
{
    public enum EolConversion
    {
        [EolBytes(ByteCode.CR)]
        CR,

        [EolBytes(ByteCode.LF)]
        LF,

        [EolBytes(ByteCode.CR, ByteCode.LF)]
        CRLF
    }
}
