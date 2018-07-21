using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EolConverter.ComponentModel
{
    public class NumBytesPerUnitAttribute : Attribute
    {
        public int NumBytesPerUnit { get; set; }

        public NumBytesPerUnitAttribute(int numBytesPerUnit)
        {
            NumBytesPerUnit = numBytesPerUnit;
        }
    }
}
