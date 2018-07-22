using System;

namespace EolConverter.ComponentModel
{
    public class EolBytesAttribute : Attribute
    {
        public byte[] EolBytes { get; set; }

        public EolBytesAttribute(params byte[] eolBytes)
        {
            EolBytes = eolBytes;
        }
    }
}
