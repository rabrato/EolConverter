using EolConverter.ByteUtils;
using EolConverter.Encoding;
using System;
using System.Linq;

namespace EolConverter
{
    public class EolConverter
    {
        private readonly EolConversion eolConversion;
        private readonly IEncodingDetector encodingDetector;

        public EolConverter(EolConversion eolConversion, IEncodingDetector encodingDetector)
        {
            this.eolConversion = eolConversion;
            this.encodingDetector = encodingDetector;
        }

        public void Convert(byte[] data, int dataLength, byte[] outputData, out int outputLength)
        {
            if (outputData == null)
            {
                throw new ArgumentNullException(nameof(outputData), $"The {nameof(outputData)} array will hold the result so it can't be null");
            }

            if (data == null || data.Length == 0 || dataLength == 0)
            {
                Array.Clear(outputData, 0, outputData.Length);
                outputLength = 0;
                return;
            }

            if (!data.HasAnyEol())
            {
                CopyInputDataToOutput(data, dataLength, outputData, out outputLength);
                return;
            }

            (var encoding, bool hasBom) = encodingDetector.GetEncoding(data, dataLength);
            if (encoding == EncodingType.None)
            {
                CopyInputDataToOutput(data, dataLength, outputData, out outputLength);
                return;
            }

            Convert(data, dataLength, encoding, hasBom, outputData, out outputLength);
        }

        private void CopyInputDataToOutput(byte[] data, int dataLength, byte[] outputData, out int outputLength)
        {
            int outputIndex = 0;
            CopyDataToOutput(data.Take(dataLength).ToArray(), outputData, ref outputIndex);
            outputLength = outputIndex;
        }

        private void Convert(byte[] data, int dataLength, EncodingType encoding, bool hasBom, byte[] outputData, out int outputLength)
        {
            int outputIndex = 0;
            if (hasBom)
            {
                CopyBomTo(outputData, encoding, out outputIndex);
            }

            int numBytesPerUnit = encoding.GetNumBytesPerUnit();
            int startDataIndex = outputIndex;
            for (int dataIndex = startDataIndex; dataIndex <= dataLength - numBytesPerUnit; dataIndex += numBytesPerUnit)
            {
                var currentUnit = GetUnit(data, dataIndex, numBytesPerUnit);
                var nextUnit = GetUnit(data, dataIndex + numBytesPerUnit, numBytesPerUnit);

                ProcessUnit(outputData, currentUnit, encoding, ref outputIndex);

                if (currentUnit.IsCR(encoding) && nextUnit.IsLF(encoding))
                {
                    // If CRLF then the do not process the next LF unit because the end of line has been inserted while processing the CR
                    dataIndex += numBytesPerUnit;
                }
            }

            outputLength = outputIndex;
        }

        private void CopyBomTo(byte[] outputData, EncodingType encoding, out int outputIndex)
        {
            var bom = encoding.GetByteOrderMark();
            outputIndex = 0;
            CopyDataToOutput(bom, outputData, ref outputIndex);
        }

        private byte[] GetUnit(byte[] data, int unitIndex, int numBytesPerUnit)
        {
            bool canTakeEntireUnit = unitIndex <= data.Length - numBytesPerUnit;
            int bytesToTake = canTakeEntireUnit ? numBytesPerUnit : data.Length - unitIndex;
            return data.Skip(unitIndex).Take(bytesToTake).ToArray();
        }

        private void ProcessUnit(byte[] outputData, byte[] unit, EncodingType encoding, ref int outputIndex)
        {
            var newEol = eolConversion.GetEolUnits(encoding);
            // If unit is an eol then copy the new eol (make the eol conversion), otherwise just copy the source data
            byte[] unitToCopy = unit.IsEol(encoding) ? newEol : unit;
            CopyDataToOutput(unitToCopy, outputData, ref outputIndex);
        }

        private void CopyDataToOutput(byte[] dataToCopy, byte[] outputData, ref int outputIndex)
        {
            if (outputData.Length < outputIndex + dataToCopy.Length)
            {
                throw new ArgumentException($"The result array does not have enough space to store the result", nameof(outputData));
            }

            dataToCopy.CopyTo(outputData, outputIndex);
            outputIndex += dataToCopy.Length;
        }
    }
}
