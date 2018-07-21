using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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