using EolConverter.ByteUtils;
using EolConverter.Encoding;
using EolConverter.Test.TestUtils;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace EolConverter.Test
{
    public class EolConverterTest
    {
        private const int BufferSize = 1024;

        byte[] outputData = new byte[BufferSize];
        int outputLength;

        Mock<IEncodingDetector> encodingDetectorMock;
        EolConverter sut;

        [Theory, MemberData(nameof(TestScenarios))]
        public void Convert(EncodingType encoding, bool hasBom, byte[] data, EolConversion conversion, byte[] expectedOutput)
        {
            // Arrange
            SetupSut(conversion, encoding, hasBom);

            // Act
            sut.Convert(data, data.Length, outputData, out outputLength);

            // Assert
            AssertOutput(expectedOutput, expectedOutput.Length, outputData, outputLength);
        }

        [Theory, MemberData(nameof(TestScenarios))]
        public void Convert_WhenDataIsEndedByNullBytes(EncodingType encoding, bool hasBom, byte[] data, EolConversion conversion, byte[] expectedOutput)
        {
            // Arrange
            var dataNullBytesEnded = new byte[BufferSize];
            data.CopyTo(dataNullBytesEnded, 0);
            SetupSut(conversion, encoding, hasBom);

            // Act
            sut.Convert(data, data.Length, outputData, out outputLength);

            // Assert
            AssertOutput(expectedOutput, expectedOutput.Length, outputData, outputLength);
        }

        [Fact]
        public void Convert_WhenOutputDataIsNull_ThenThrowArgumentNullException()
        {
            // Arrange
            byte[] data = new byte[3] { 1, 1, 1};
            int dataLength = 3;
            byte[] outputData = null;
            SetupSutWithDummyData();

            // Act + Assert
            Assert.Throws<ArgumentNullException>("outputData", () => sut.Convert(data, dataLength, outputData, out outputLength));
        }

        [Fact]
        public void Convert_WhenOutputDataDoesNotHaveEnoughSpace_ThenThrowArgumentException()
        {
            // Arrange
            byte[] data = new byte[1] { ByteCode.CR };
            int dataLength = 1;
            SetupSut(EolConversion.CRLF, EncodingType.Utf8, hasBom: false);

            // CRs will be converted to CRLF so the outputdata will need more space than data
            byte[] outputData = new byte[data.Length];

            // Act + Assert
            Assert.Throws<ArgumentException>("outputData", () => sut.Convert(data, dataLength, outputData, out outputLength));
        }

        [Fact]
        public void Convert_WhenOutputDataHasExactlyRequiredSpace()
        {
            // Arrange
            byte[] data = new byte[1] { ByteCode.CR };
            int dataLength = 1;
            SetupSut(EolConversion.LF, EncodingType.Utf8, hasBom: false);

            // CRs will be converted to CRLF so the outputdata will need more space than data
            byte[] outputData = new byte[data.Length];

            // Act
            sut.Convert(data, dataLength, outputData, out outputLength);

            // Assert
            AssertOutput(new byte[1] { ByteCode.LF }, dataLength, outputData, outputLength);
        }

        [Fact]
        public void Convert_WhenDataIsNull_ThenReturnEmptyResult()
        {
            // Arrange
            byte[] data = null;
            int dataLength = 0;
            SetupSutWithDummyData();

            // Act
            sut.Convert(data, dataLength, outputData, out outputLength);

            // Assert
            AssertEmptyOutput(outputData, outputLength);
        }

        [Fact]
        public void Convert_WhenDataIsEmpty_ThenReturnEmptyResult()
        {
            // Arrange
            byte[] emptyData = new byte[0];
            int dataLength = 0;
            SetupSutWithDummyData();

            // Act
            sut.Convert(emptyData, dataLength, outputData, out outputLength);

            // Assert
            AssertEmptyOutput(outputData, outputLength);
        }

        [Fact]
        public void Convert_WhenDataLengthIsZero_ThenReturnEmptyResult()
        {
            // Arrange
            byte[] dummyData = new byte[3] { ByteCode.CR, 1, 1 };
            int dataLength = 0;
            SetupSutWithDummyData();

            // Act
            sut.Convert(dummyData, dataLength, outputData, out outputLength);

            // Assert
            AssertEmptyOutput(outputData, outputLength);
        }

        [Fact]
        public void Convert_WhenDataDoesNotHaveAnyEol_ThenReturnInput()
        {
            // Arrange
            byte[] dataWithoutEol = new byte[3] { 1, 1, 1 };
            SetupSutWithDummyData();

            // Act
            sut.Convert(dataWithoutEol, dataWithoutEol.Length, outputData, out outputLength);

            // Assert
            AssertOutput(dataWithoutEol, dataWithoutEol.Length, outputData, outputLength);
        }

        [Fact]
        public void Convert_WhenEncodingNotDetected_ThenReturnInput()
        {
            // Arrange
            byte[] data = new byte[3] { ByteCode.CR, 1, 1 };
            SetupSut(EolConversion.CR, EncodingType.None, hasBom: false);

            // Act
            sut.Convert(data, data.Length, outputData, out outputLength);

            // Assert
            AssertOutput(data, data.Length, outputData, outputLength);
        }

        private void AssertEmptyOutput(byte[] outputData, int outputLength)
        {
            AssertOutput(new byte[0], 0, outputData, outputLength);
        }

        private void AssertOutput(byte[] expectedData, int expectedDataLength, byte[] outputData, int outputLength)
        {
            Assert.Equal(expectedDataLength, outputLength);
            Assert.Equal(expectedData, outputData?.Take(outputLength));
        }

        public static IEnumerable<object[]> TestScenarios => EolConversionTestScenarios.TestScenarios;

        private void SetupSut(EolConversion eolConversion, EncodingType encoding, bool hasBom)
        {
            encodingDetectorMock = IEncodingDetectorMockBuilder.Create()
                .WithDetectedEncoding(encoding, hasBom);
            CreateSut(eolConversion);
        }

        private void SetupSutWithDummyData()
        {
            SetupSut(EolConversion.CR, EncodingType.None, false);
        }

        private void CreateSut(EolConversion eolConversion)
        {
            sut = new EolConverter(eolConversion, encodingDetectorMock.Object);
        }
    }
}
