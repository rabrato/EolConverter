using EolConverter.ByteUtils;
using EolConverter.ComponentModel;

namespace EolConverter
{
    public enum EolConversion
    {
        [EolBytes(ByteCode.Cr)]
        CR,

        [EolBytes(ByteCode.Lf)]
        LF,

        [EolBytes(ByteCode.Cr, ByteCode.Lf)]
        CRLF
    }
}
