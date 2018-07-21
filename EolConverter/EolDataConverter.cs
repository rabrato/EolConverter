using EolConverter.Encoding;
using EolConverter.EolConverters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EolConverter
{
    public class EolDataConverter
    {
        EolConversion eolConversion;
        EncodingDetector encodingDetector;

        public EolDataConverter(EolConversion eolConversion)
        {
            this.eolConversion = eolConversion;
            encodingDetector = new EncodingDetector();
        }

        public void Convert(byte[] data, int dataLength, byte[] outputData, out int outputLength)
        {
            if (data == null || dataLength == 0 || !data.HasAnyEndOfLine())
            {
                outputData = data;
                outputLength = dataLength;
                return;
            }

            var encoding = encodingDetector.GetEncoding(data, dataLength);
            int numBytesPerUnit = encoding.GetNumBytesPerUnit();
            int bomLength = encoding.GetByteOrderMark().Length;

            int outputIndex = 0;
            for (int unitIndex = bomLength; unitIndex <= dataLength - numBytesPerUnit; unitIndex += numBytesPerUnit)
            {
                var currentUnit = GetUnit(data, unitIndex, numBytesPerUnit);
                var nextUnit = GetUnit(data, unitIndex + numBytesPerUnit, numBytesPerUnit);

                if (currentUnit.IsEol(encoding))
                {
                    ReplaceEndOfLine(currentUnit, nextUnit, encoding, outputData, outputIndex);
                }
                else
                {
                    outputData.CopyUnitAt(currentUnit, outputIndex);
                }

                if (IsCrLf(currentUnit, nextUnit, encoding) && eolConversion != EolConversion.CRLF)
                {
                    // CRLF (2 units) has been replace by CR or LF (1 unit) 
                    // so the next data unit (the LF of CRLF) must be skipped because it has been already processed
                    unitIndex += numBytesPerUnit;
                }

                outputIndex += numBytesPerUnit;
            }

            outputLength = outputIndex;
        }

        private void ReplaceEndOfLine(byte[] currentUnit, byte[] nextUnit, EncodingType encoding, byte[] outputData, int outputIndex)
        {
            if (IsCrLf(currentUnit, nextUnit, encoding))
            {
                outputData.CopyCrAt(outputIndex, encoding);
            }
            else if (currentUnit.IsCr(encoding))
            {
                // do nothing
            }
            else if (currentUnit.IsLf(encoding))
            {
                outputData.CopyCrAt(outputIndex, encoding);
            }
        }

        private bool IsCrLf(byte[] currentUnit, byte[] nextUnit, EncodingType encoding)
        {
            return currentUnit.IsCr(encoding) && nextUnit.IsLf(encoding);
        }

        private byte[] GetUnit(byte[] data, int unitIndex, int numBytesPerUnit)
        {
            bool canTakeEntireUnit = unitIndex <= data.Length - numBytesPerUnit;
            int bytesToTake = canTakeEntireUnit ? numBytesPerUnit : data.Length - unitIndex;
            return data.Skip(unitIndex).Take(bytesToTake).ToArray();
        }
    }
}
