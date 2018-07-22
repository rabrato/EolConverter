using EolConverter.ByteUtils;
using EolConverter.Encoding;
using System;
using System.Linq;

namespace EolConverter
{
    public class EolConverter
    {
        EolConversion eolConversion;
        IEncodingDetector encodingDetector;

        public EolConverter(EolConversion eolConversion, IEncodingDetector encodingDetector)
        {
            this.eolConversion = eolConversion;
            this.encodingDetector = encodingDetector;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="dataLength"></param>
        /// <param name="outputData"></param>
        /// <param name="outputLength"></param>
        public void Convert(byte[] data, int dataLength, byte[] outputData, out int outputLength)
        {
            if (outputData == null)
            {
                throw new ArgumentNullException(nameof(outputData), $"The {nameof(outputData)} array will hold the result so it can't be null");
            }

            //if (outputData.Length < dataLength)
            //{
            //    throw new ArgumentException($"The {nameof(outputData)} array does not have enough space to store the result", nameof(outputData));
            //}

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
            data.CopyTo(outputData, 0);
            outputLength = dataLength;
        }

        private void Convert(byte[] data, int dataLength, EncodingType encoding, bool hasBom, byte[] outputData, out int outputLength)
        {
            int numBytesPerUnit = encoding.GetNumBytesPerUnit();
            var targetEolUnits = eolConversion.GetEolUnits(encoding);

            int outputIndex = 0;
            if (hasBom)
            {
                CopyBomTo(outputData, encoding, ref outputIndex);
            }

            int startDataIndex = outputIndex;
            for (int dataIndex = startDataIndex; dataIndex <= dataLength - numBytesPerUnit; dataIndex += numBytesPerUnit)
            {
                var currentUnit = GetUnit(data, dataIndex, numBytesPerUnit);
                var nextUnit = GetUnit(data, dataIndex + numBytesPerUnit, numBytesPerUnit);

                ProcessUnit(outputData, currentUnit, targetEolUnits, encoding, ref outputIndex);

                if (currentUnit.IsCrUnit(encoding) && nextUnit.IsLfUnit(encoding))
                {
                    // If CRLF then the do not process the next LF unit because the end of line has been inserted while processing the CR
                    dataIndex += numBytesPerUnit;
                }
            }

            outputLength = outputIndex;
        }

        private void CopyBomTo(byte[] outputData, EncodingType encoding, ref int outputIndex)
        {
            var bom = encoding.GetByteOrderMark();
            bom.CopyTo(outputData, 0);
            outputIndex += bom.Length;
        }

        private byte[] GetUnit(byte[] data, int unitIndex, int numBytesPerUnit)
        {
            bool canTakeEntireUnit = unitIndex <= data.Length - numBytesPerUnit;
            int bytesToTake = canTakeEntireUnit ? numBytesPerUnit : data.Length - unitIndex;
            return data.Skip(unitIndex).Take(bytesToTake).ToArray();
        }

        private void ProcessUnit(byte[] data, byte[] unit, byte[] targetEolUnits, EncodingType encoding, ref int dataIndex)
        {
            byte[] unitToCopy = unit.IsEolUnit(encoding) ? targetEolUnits : unit;

            unitToCopy.CopyTo(data, dataIndex);
            dataIndex += unitToCopy.Length;
        }
    }
}
