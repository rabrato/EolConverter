using EolConverter.ByteUtils;
using EolConverter.Encoding;
using System.Collections.Generic;
using System.Linq;

namespace EolConverter.Test.TestUtils
{
    internal class EolConversionTestScenarios
    {
        private static byte[] data = new byte[11] { 1, ByteCode.CR, 1, ByteCode.LF, 1, ByteCode.LF, ByteCode.CR, 1, ByteCode.CR, ByteCode.LF, 1 };
        private static byte[] dataCRConverted = new byte[10] { 1, ByteCode.CR, 1, ByteCode.CR, 1, ByteCode.CR, ByteCode.CR, 1, ByteCode.CR, 1 };
        private static byte[] dataLFConverted = new byte[10] { 1, ByteCode.LF, 1, ByteCode.LF, 1, ByteCode.LF, ByteCode.LF, 1, ByteCode.LF, 1 };
        private static byte[] dataCRLFConverted = new byte[15] { 1, ByteCode.CR, ByteCode.LF, 1, ByteCode.CR, ByteCode.LF, 1, ByteCode.CR, ByteCode.LF, ByteCode.CR, ByteCode.LF, 1, ByteCode.CR, ByteCode.LF, 1 };

        private static byte[] dataStartByCR = new byte[11] { ByteCode.CR, 1, 1, ByteCode.LF, 1, ByteCode.LF, ByteCode.CR, 1, ByteCode.CR, ByteCode.LF, 1 };
        private static byte[] dataStartByLF = new byte[11] { ByteCode.LF, 1, 1, ByteCode.CR, 1, ByteCode.LF, ByteCode.CR, 1, ByteCode.CR, ByteCode.LF, 1 };
        private static byte[] dataStartByCRLF = new byte[12] { ByteCode.CR, ByteCode.LF, 1, 1, ByteCode.LF, 1, ByteCode.LF, ByteCode.CR, 1, ByteCode.CR, ByteCode.LF, 1 };
        private static byte[] dataStartCRConverted = new byte[10] { ByteCode.CR, 1, 1, ByteCode.CR, 1, ByteCode.CR, ByteCode.CR, 1, ByteCode.CR, 1 };
        private static byte[] dataStartLFConverted = new byte[10] { ByteCode.LF, 1, 1, ByteCode.LF, 1, ByteCode.LF, ByteCode.LF, 1, ByteCode.LF, 1 };
        private static byte[] dataStartCRLFConverted = new byte[15] { ByteCode.CR, ByteCode.LF, 1, 1, ByteCode.CR, ByteCode.LF, 1, ByteCode.CR, ByteCode.LF, ByteCode.CR, ByteCode.LF, 1, ByteCode.CR, ByteCode.LF, 1 };

        private static byte[] dataEndByCR = new byte[10] { 1, ByteCode.LF, 1, ByteCode.LF, ByteCode.CR, 1, ByteCode.CR, ByteCode.LF, 1, ByteCode.CR };
        private static byte[] dataEndByLF = new byte[10] { 1, ByteCode.CR, 1, ByteCode.LF, ByteCode.CR, 1, ByteCode.CR, ByteCode.LF, 1, ByteCode.LF };
        private static byte[] dataEndByCRLF = new byte[11] { 1, ByteCode.CR, 1, ByteCode.LF, ByteCode.CR, 1, ByteCode.CR, ByteCode.LF, 1, ByteCode.CR, ByteCode.LF };
        private static byte[] dataEndCRConverted = new byte[9] { 1, ByteCode.CR, 1, ByteCode.CR, ByteCode.CR, 1, ByteCode.CR, 1, ByteCode.CR };
        private static byte[] dataEndLFConverted = new byte[9] { 1, ByteCode.LF, 1, ByteCode.LF, ByteCode.LF, 1, ByteCode.LF, 1, ByteCode.LF };
        private static byte[] dataEndCRLFConverted = new byte[14] { 1, ByteCode.CR, ByteCode.LF, 1, ByteCode.CR, ByteCode.LF, ByteCode.CR, ByteCode.LF, 1, ByteCode.CR, ByteCode.LF, 1, ByteCode.CR, ByteCode.LF };

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
            (data, EolConversion.CR, dataCRConverted),
            (data, EolConversion.LF, dataLFConverted),
            (data, EolConversion.CRLF, dataCRLFConverted),

            (dataStartByCR, EolConversion.CR, dataStartCRConverted),
            (dataStartByLF, EolConversion.CR, dataStartCRConverted),
            (dataStartByCRLF, EolConversion.CR, dataStartCRConverted),

            (dataStartByCR, EolConversion.LF, dataStartLFConverted),
            (dataStartByLF, EolConversion.LF, dataStartLFConverted),
            (dataStartByCRLF, EolConversion.LF, dataStartLFConverted),

            (dataStartByCR, EolConversion.CRLF, dataStartCRLFConverted),
            (dataStartByLF, EolConversion.CRLF, dataStartCRLFConverted),
            (dataStartByCRLF, EolConversion.CRLF, dataStartCRLFConverted),

            (dataEndByCR, EolConversion.CR, dataEndCRConverted),
            (dataEndByLF, EolConversion.CR, dataEndCRConverted),
            (dataEndByCRLF, EolConversion.CR, dataEndCRConverted),
                 
            (dataEndByCR, EolConversion.LF, dataEndLFConverted),
            (dataEndByLF, EolConversion.LF, dataEndLFConverted),
            (dataEndByCRLF, EolConversion.LF, dataEndLFConverted),
                 
            (dataEndByCR, EolConversion.CRLF, dataEndCRLFConverted),
            (dataEndByLF, EolConversion.CRLF, dataEndCRLFConverted),
            (dataEndByCRLF, EolConversion.CRLF, dataEndCRLFConverted),
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
