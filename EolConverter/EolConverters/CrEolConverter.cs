using EolConverter.Encoding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EolConverter.EolConverters
{
    internal class CrEolConverter : IEolConverter
    {
        EncodingDetector encodingDetector;

        public CrEolConverter(EncodingDetector encodingDetector)
        {
            this.encodingDetector = encodingDetector;
        }

        public void Convert(byte[] data, int dataLength, byte[] outputData, out int outputLength)
        {
            if (data == null || dataLength == 0 || !data.HasAnyEndOfLine())
            {
                outputData = data;
                outputLength =  dataLength;
                return;
            }

            var encoding = encodingDetector.GetEncoding(data, dataLength);
            int numBytesPerUnit = encoding.GetNumBytesPerUnit();

            int bomLength = encoding.GetByteOrderMark().Length;
            int outputIndex = 0;
            int lastUnitIndex = dataLength - numBytesPerUnit;
            for (int unitIndex = bomLength; unitIndex <= lastUnitIndex; unitIndex += numBytesPerUnit)
            {
                var currentUnit = data.Skip(unitIndex).Take(numBytesPerUnit).ToArray();
                int nextUnitIndex = unitIndex + numBytesPerUnit;
                var nextUnit = nextUnitIndex <= lastUnitIndex ? 
                    data.Skip(nextUnitIndex).Take(numBytesPerUnit).ToArray()
                    : null;

                if (currentUnit.IsCr(encoding) && nextUnit?.IsLf(encoding) == true)
                {
                    outputData.CopyCrAt(outputIndex, encoding);
                    unitIndex += numBytesPerUnit;
                }
                else if (currentUnit.IsCr(encoding))
                {
                    // do nothing
                }
                else if (currentUnit.IsLf(encoding))
                {
                    outputData.CopyCrAt(outputIndex, encoding);
                }
                else
                {
                    outputData.CopyCharAt(currentUnit, outputIndex);
                }

                outputIndex += numBytesPerUnit;
            }

            outputLength = outputIndex;
        }
    }
}
