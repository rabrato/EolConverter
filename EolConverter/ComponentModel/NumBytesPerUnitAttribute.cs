using System;

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
