using EolConverter.Encoding;
using EolConverter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EolConverter.ByteUtils;

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

            (var encoding, bool hasBom) = encodingDetector.GetEncoding(data, dataLength);
            int numBytesPerUnit = encoding.GetNumBytesPerUnit();
            int initialIndex = hasBom ? encoding.GetByteOrderMark().Length : 0;

            int outputIndex = 0;
            for (int unitIndex = initialIndex; unitIndex <= dataLength - numBytesPerUnit; unitIndex += numBytesPerUnit)
            {
                var currentUnit = GetUnit(data, unitIndex, numBytesPerUnit);
                var nextUnit = GetUnit(data, unitIndex + numBytesPerUnit, numBytesPerUnit);

                ProcessUnit(outputData, currentUnit, encoding, ref outputIndex);

                if (currentUnit.IsCr(encoding) && nextUnit.IsLf(encoding))
                {
                    // If CRLF then the do not process the next LF unit because the end of line has been inserted while processing the CR
                    unitIndex += numBytesPerUnit;
                }
            }

            outputLength = outputIndex;
        }

        private byte[] GetUnit(byte[] data, int unitIndex, int numBytesPerUnit)
        {
            bool canTakeEntireUnit = unitIndex <= data.Length - numBytesPerUnit;
            int bytesToTake = canTakeEntireUnit ? numBytesPerUnit : data.Length - unitIndex;
            return data.Skip(unitIndex).Take(bytesToTake).ToArray();
        }

        private void ProcessUnit(byte[] outputData, byte[] unit, EncodingType encoding, ref int outputIndex)
        {
            if (unit.IsEol(encoding))
            {
                InsertTargetEol(outputData, encoding, ref outputIndex);
            }
            else
            {
                CopyUnit(outputData, unit, ref outputIndex);
            }
        }

        private void InsertTargetEol(byte[] data, EncodingType encoding, ref int dataIndex)
        { 
            byte[] targetEolChars = eolConversion.GetEolBytes();
            byte[] targetEolUnits = targetEolChars
                .Select(eolChar => CharacterUtils.GetCharUnit(eolChar, encoding))
                .Aggregate((eolUnit1, eolUnit2) => eolUnit1.Concat(eolUnit2).ToArray());

            data.CopyUnitAt(targetEolUnits, dataIndex);
            dataIndex += targetEolUnits.Length;
        }

        private void CopyUnit(byte[] data, byte[] unit, ref int dataIndex)
        {
            data.CopyUnitAt(unit, dataIndex);
            dataIndex += unit.Length;
        }
    }
}
