using EolConverter.ByteUtils;
using EolConverter.Encoding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EolConverter.Test.TestUtils
{
    internal class EolConversionTestScenarios
    {
        private static byte[] data = new byte[11] { 1, ByteCode.Cr, 1, ByteCode.Lf, 1, ByteCode.Lf, ByteCode.Cr, 1, ByteCode.Cr, ByteCode.Lf, 1 };
        private static byte[] dataCrConverted = new byte[10] { 1, ByteCode.Cr, 1, ByteCode.Cr, 1, ByteCode.Cr, ByteCode.Cr, 1, ByteCode.Cr, 1 };
        private static byte[] dataLfConverted = new byte[10] { 1, ByteCode.Lf, 1, ByteCode.Lf, 1, ByteCode.Lf, ByteCode.Lf, 1, ByteCode.Lf, 1 };
        private static byte[] dataCrLfConverted = new byte[15] { 1, ByteCode.Cr, ByteCode.Lf, 1, ByteCode.Cr, ByteCode.Lf, 1, ByteCode.Cr, ByteCode.Lf, ByteCode.Cr, ByteCode.Lf, 1, ByteCode.Cr, ByteCode.Lf, 1 };

        private static byte[] dataStartByCr = new byte[11] { ByteCode.Cr, 1, 1, ByteCode.Lf, 1, ByteCode.Lf, ByteCode.Cr, 1, ByteCode.Cr, ByteCode.Lf, 1 };
        private static byte[] dataStartByLf = new byte[11] { ByteCode.Lf, 1, 1, ByteCode.Cr, 1, ByteCode.Lf, ByteCode.Cr, 1, ByteCode.Cr, ByteCode.Lf, 1 };
        private static byte[] dataStartByCrLf = new byte[12] { ByteCode.Cr, ByteCode.Lf, 1, 1, ByteCode.Lf, 1, ByteCode.Lf, ByteCode.Cr, 1, ByteCode.Cr, ByteCode.Lf, 1 };
        private static byte[] dataStartCrConverted = new byte[10] { ByteCode.Cr, 1, 1, ByteCode.Cr, 1, ByteCode.Cr, ByteCode.Cr, 1, ByteCode.Cr, 1 };
        private static byte[] dataStartLfConverted = new byte[10] { ByteCode.Lf, 1, 1, ByteCode.Lf, 1, ByteCode.Lf, ByteCode.Lf, 1, ByteCode.Lf, 1 };
        private static byte[] dataStartCrLfConverted = new byte[15] { ByteCode.Cr, ByteCode.Lf, 1, 1, ByteCode.Cr, ByteCode.Lf, 1, ByteCode.Cr, ByteCode.Lf, ByteCode.Cr, ByteCode.Lf, 1, ByteCode.Cr, ByteCode.Lf, 1 };

        private static byte[] dataEndByCr = new byte[10] { 1, ByteCode.Lf, 1, ByteCode.Lf, ByteCode.Cr, 1, ByteCode.Cr, ByteCode.Lf, 1, ByteCode.Cr };
        private static byte[] dataEndByLf = new byte[10] { 1, ByteCode.Cr, 1, ByteCode.Lf, ByteCode.Cr, 1, ByteCode.Cr, ByteCode.Lf, 1, ByteCode.Lf };
        private static byte[] dataEndByCrLf = new byte[11] { 1, ByteCode.Cr, 1, ByteCode.Lf, ByteCode.Cr, 1, ByteCode.Cr, ByteCode.Lf, 1, ByteCode.Cr, ByteCode.Lf };
        private static byte[] dataEndCrConverted = new byte[9] { 1, ByteCode.Cr, 1, ByteCode.Cr, ByteCode.Cr, 1, ByteCode.Cr, 1, ByteCode.Cr };
        private static byte[] dataEndLfConverted = new byte[9] { 1, ByteCode.Lf, 1, ByteCode.Lf, ByteCode.Lf, 1, ByteCode.Lf, 1, ByteCode.Lf };
        private static byte[] dataEndCrLfConverted = new byte[14] { 1, ByteCode.Cr, ByteCode.Lf, 1, ByteCode.Cr, ByteCode.Lf, ByteCode.Cr, ByteCode.Lf, 1, ByteCode.Cr, ByteCode.Lf, 1, ByteCode.Cr, ByteCode.Lf };

        public static IEnumerable<object[]> TestScenarios => 
            EncodingsToTest.SelectMany(encoding =>
                DataScenarios.SelectMany(dataScenario => 
                    CreateTestScenario(encoding, dataScenario.input, dataScenario.conversion, dataScenario.output)
                )
            );

        private static IEnumerable<EncodingType> EncodingsToTest => new List<EncodingType>()
            {
                EncodingType.Utf8,
                EncodingType.Utf16BE,
                EncodingType.Utf16LE,
                EncodingType.Utf32BE,
                EncodingType.Utf32LE,
            };

        private static IEnumerable<(byte[] input, EolConversion conversion, byte[] output)> DataScenarios => 
            new List<(byte[], EolConversion, byte[])>
        {
            (data, EolConversion.CR, dataCrConverted),
            (data, EolConversion.LF, dataLfConverted),
            (data, EolConversion.CRLF, dataCrLfConverted),

            (dataStartByCr, EolConversion.CR, dataStartCrConverted),
            (dataStartByLf, EolConversion.CR, dataStartCrConverted),
            (dataStartByCrLf, EolConversion.CR, dataStartCrConverted),

            (dataStartByCr, EolConversion.LF, dataStartLfConverted),
            (dataStartByLf, EolConversion.LF, dataStartLfConverted),
            (dataStartByCrLf, EolConversion.LF, dataStartLfConverted),

            (dataStartByCr, EolConversion.CRLF, dataStartCrLfConverted),
            (dataStartByLf, EolConversion.CRLF, dataStartCrLfConverted),
            (dataStartByCrLf, EolConversion.CRLF, dataStartCrLfConverted),

            (dataEndByCr, EolConversion.CR, dataEndCrConverted),
            (dataEndByLf, EolConversion.CR, dataEndCrConverted),
            (dataEndByCrLf, EolConversion.CR, dataEndCrConverted),
                 
            (dataEndByCr, EolConversion.LF, dataEndLfConverted),
            (dataEndByLf, EolConversion.LF, dataEndLfConverted),
            (dataEndByCrLf, EolConversion.LF, dataEndLfConverted),
                 
            (dataEndByCr, EolConversion.CRLF, dataEndCrLfConverted),
            (dataEndByLf, EolConversion.CRLF, dataEndCrLfConverted),
            (dataEndByCrLf, EolConversion.CRLF, dataEndCrLfConverted),
        };

        private static IEnumerable<object[]> CreateTestScenario(EncodingType encoding, byte[] input, EolConversion conversion, byte[] output)
        {
            var bom = encoding.GetByteOrderMark();
            var inputInUnits = input.ToUnits(encoding);
            var outputInUnits = output.ToUnits(encoding);
            var inputInUnitsWithBom = bom.Concat(inputInUnits).ToArray();
            var outputInUnitsWithBom = bom.Concat(outputInUnits).ToArray();
            return new List<object[]>
            {
                new object[] { encoding, false, inputInUnits, conversion, outputInUnits },
                new object[] { encoding, true, inputInUnitsWithBom, conversion, outputInUnitsWithBom },
            };
        }
    }
}
