using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
