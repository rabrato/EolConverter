using System;

namespace EolConverter.ComponentModel
{
    public class ByteOrderMarkAttribute : Attribute
    {
        public byte[] ByteOrderMark { get; set; }

        public ByteOrderMarkAttribute(params byte[] byteOrderMark)
        {
            ByteOrderMark = byteOrderMark;
        }
    }
}