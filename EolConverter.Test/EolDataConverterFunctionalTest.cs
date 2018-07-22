using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Xunit;

namespace EolConverter.Test
{
    public class EolDataConverterFunctionalTest
    {
        const int BlockSize = 1024;

        EolDataConverter sut;

        [Theory, MemberData(nameof(ConversionScenarios))]
        public void Convert(string inputFile, EolConversion eolConversion, string expectedOutputFile)
        {
            // Arrange
            byte[] inputBuffer = new byte[BlockSize];
            byte[] expectedOutputBuffer = new byte[BlockSize * 2];
            byte[] outputBuffer = new byte[BlockSize * 2];

            sut = new EolDataConverter(eolConversion);

            using (var reader = GetTestFileStream(inputFile))
            using (var expectedOutputReader = GetTestFileStream(expectedOutputFile))
            {
                int outputLength;
                int expectedOutputRead;
                int inputRead = 0;
                while ((inputRead = reader.Read(inputBuffer, 0, BlockSize)) != 0)
                {
                    // Act
                    sut.Convert(inputBuffer, inputRead, outputBuffer, out outputLength);

                    // Assert
                    expectedOutputRead = expectedOutputReader.Read(expectedOutputBuffer, 0, outputLength);
                    Assert.Equal(expectedOutputRead, outputLength);
                    Assert.Equal(expectedOutputBuffer, outputBuffer);
                }

                // Check expected output does not have more bytes to read, otherwise it will be different from converted input that has ended
                expectedOutputRead = expectedOutputReader.Read(expectedOutputBuffer, 0, BlockSize);
                Assert.Equal(0, expectedOutputRead);
            }
        }

        public static IEnumerable<object[]> ConversionScenarios =>
            InputFiles.SelectMany(inputFile =>
                Conversions.Select(conversion => CreateConversionScenario(inputFile, conversion)
                )
            );

        private static object[] CreateConversionScenario(string inputFile, EolConversion conversion)
        {
            return new object[3] { inputFile, conversion, $"{inputFile}_{conversion.ToString()}" };
        }

        private static IEnumerable<string> InputFiles => new List<string>()
        {
            "Utf8.Utf8Bom",
            "Utf8.Utf8NoBom",
            "Utf16LE.Utf16LEBom",
            "Utf16LE.Utf16LENoBom",
            "Utf16BE.Utf16BEBom",
            "Utf16BE.Utf16BENoBom",
            "Utf32LE.Utf32LEBom",
            "Utf32LE.Utf32LENoBom",
            "Utf32BE.Utf32BEBom",
            "Utf32BE.Utf32BENoBom",
        };

        private static IEnumerable<EolConversion> Conversions => new List<EolConversion>()
        {
            EolConversion.CR,
            EolConversion.LF,
            EolConversion.CRLF
        };

        private Stream GetTestFileStream(string testFileName)
        {
            string filePath = $"EolConverter.Test.TestUtils.TestFiles.{testFileName}.txt";
            return Assembly.GetExecutingAssembly().GetManifestResourceStream(filePath);
        }
    }
}
