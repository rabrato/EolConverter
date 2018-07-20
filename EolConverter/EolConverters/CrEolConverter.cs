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
        public void Convert(byte[] data, int dataLength, byte[] outputData, out int outputLength)
        {
            if (data == null || dataLength == 0 || !data.HasAnyEndOfLine())
            {
                outputData = data;
                outputLength =  dataLength;
                return;
            }

            var encoding = data.GetEncoding(dataLength);
            int numBytesPerUnit = encoding.GetNumBytesPerUnit();

            int outputIndex = 0;
            for (int dataIndex = 0; dataIndex < dataLength - numBytesPerUnit; dataIndex += numBytesPerUnit)
            {
                var currentChar = data.Skip(dataIndex).Take(numBytesPerUnit).ToArray();
                var nextChar = data.Skip(dataIndex + numBytesPerUnit).Take(numBytesPerUnit).ToArray();

                if (currentChar.IsCr(encoding) && nextChar.IsLf(encoding))
                {
                    outputData.CopyCrAt(outputIndex, encoding);
                    dataIndex += numBytesPerUnit;
                }
                else if (currentChar.IsCr(encoding))
                {
                    // do nothing
                }
                else if (currentChar.IsLf(encoding))
                {
                    outputData.CopyCrAt(outputIndex, encoding);
                }
                else
                {
                    outputData.CopyCharAt(currentChar, outputIndex);
                }

                outputIndex += numBytesPerUnit;
            }

            // CHECK LAST CHAR

            outputLength = outputIndex;
        }
    }
}
