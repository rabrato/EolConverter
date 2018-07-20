using EolConverter.Encoding;
using EolConverter.EolConverters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace EolConverter.Test.Encoding
{
    public class EncodingDetectorTest
    {
        private const byte DummyByte = 100;
        private const int BufferSize = 1024;

        private EncodingDetector sut;

        [Theory, MemberData(nameof(BomPerUtfTestData))]
        public void GetEncoding_WhenDataHasBom(byte[] bom, EncodingType expectedEncoding)
        {
            // Arrange
            byte[] data = GetData();
            bom.CopyTo(data, 0);

            CreateSut();

            // Act
            var encoding = sut.GetEncoding(data, dataLength: bom.Length);

            // Assert
            Assert.Equal(expectedEncoding, encoding);
        }

        [Theory, MemberData(nameof(EolPerUtfTestData))]
        public void GetEncoding_WhenDataOnlyContainsEol(byte[] eolBytes, EncodingType expectedEncoding)
        {
            // Arrange
            byte[] data = GetData();
            eolBytes.CopyTo(data, 0);

            CreateSut();

            // Act
            var encoding = sut.GetEncoding(data, dataLength: eolBytes.Length);

            // Assert
            Assert.Equal(expectedEncoding, encoding);
        }

        [Theory, MemberData(nameof(EolPerUtfTestData))]
        public void GetEncoding_WhenDataStartsByEol(byte[] eolBytes, EncodingType expectedEncoding)
        {
            // Arrange
            byte[] data = GetData(initializeWithDummyValues: true);
            eolBytes.CopyTo(data, 0);

            CreateSut();

            // Act
            var encoding = sut.GetEncoding(data, dataLength: BufferSize);

            // Assert
            Assert.Equal(expectedEncoding, encoding);
        }

        [Theory, MemberData(nameof(EolPerUtfTestData))]
        public void GetEncoding_WhenDataContainsEol(byte[] eolBytes, EncodingType expectedEncoding)
        {
            // Arrange
            byte[] data = GetData(initializeWithDummyValues: true);
            eolBytes.CopyTo(data, BufferSize / 2);

            CreateSut();

            // Act
            var encoding = sut.GetEncoding(data, dataLength: BufferSize);

            // Assert
            Assert.Equal(expectedEncoding, encoding);
        }

        [Theory, MemberData(nameof(EolPerUtfTestData))]
        public void GetEncoding_WhenDataEndsByEol(byte[] eolBytes, EncodingType expectedEncoding)
        {
            // Arrange
            byte[] data = GetData(initializeWithDummyValues: true);
            eolBytes.CopyTo(data, BufferSize - eolBytes.Length);

            CreateSut();

            // Act
            var encoding = sut.GetEncoding(data, dataLength: BufferSize);

            // Assert
            Assert.Equal(expectedEncoding, encoding);
        }

        [Theory, MemberData(nameof(EolPerUtfTestData))]
        public void GetEncoding_WhenDataEndsByEolButDataIsNotCompletelyFilled(byte[] eolBytes, EncodingType expectedEncoding)
        {
            // Arrange
            byte[] data = GetData();
            int dataLength = 100;
            FillDataWithDummyValues(data, dataLength);
            eolBytes.CopyTo(data, dataLength - eolBytes.Length);

            CreateSut();

            // Act
            var encoding = sut.GetEncoding(data, dataLength);

            // Assert
            Assert.Equal(expectedEncoding, encoding);
        }

        [Theory]
        [InlineData(new byte[3] { 0, EolByte.Cr, 0 })]
        [InlineData(new byte[3] { 0, EolByte.Lf, 0 })]
        [InlineData(new byte[5] { 0, EolByte.Cr, 0, EolByte.Lf, 0 })]
        public void GetEncoding_WhenEolIsSurroundedByOneZeros_ThenCanNotDistinguishUtf16Endiannes(byte[] eolBytesZeroSurrounded)
        {
            // Arrange
            byte[] data = GetData(initializeWithDummyValues: true);
            eolBytesZeroSurrounded.CopyTo(data, 10);

            CreateSut();

            // Act
            var encoding = sut.GetEncoding(data, dataLength: BufferSize);

            // Assert
            Assert.Equal(EncodingType.None, encoding);
        }

        [Theory]
        [InlineData(new byte[7] { 0, 0, 0, EolByte.Cr, 0, 0, 0 })]
        [InlineData(new byte[7] { 0, 0, 0, EolByte.Lf, 0, 0, 0 })]
        [InlineData(new byte[11] { 0, 0, 0, EolByte.Cr, 0, 0, 0, EolByte.Lf, 0, 0, 0 })]
        public void GetEncoding_WhenEolIsSurroundedByThreeZeros_ThenCanNotDistinguishUtf32Endiannes(byte[] eolBytesZeroSurrounded)
        {
            // Arrange
            byte[] data = GetData(initializeWithDummyValues: true);
            eolBytesZeroSurrounded.CopyTo(data, 10);

            CreateSut();

            // Act
            var encoding = sut.GetEncoding(data, dataLength: BufferSize);

            // Assert
            Assert.Equal(EncodingType.None, encoding);
        }

        [Fact]
        public void GetEncoding_WhenDataIsNull()
        {
            // Arrange
            CreateSut();

            // Act
            var encoding = sut.GetEncoding(data: null, dataLength: 0);

            // Assert
            Assert.Equal(EncodingType.None, encoding);
        }

        [Fact]
        public void GetEncoding_WhenDataLengthIsZero()
        {
            // Arrange
            byte[] data = GetData();
            CreateSut();

            // Act
            var encoding = sut.GetEncoding(data, dataLength: 0);

            // Assert
            Assert.Equal(EncodingType.None, encoding);
        }

        [Fact]
        public void GetEncoding_WhenDataIsEmpty()
        {
            // Arrange
            byte[] data = new byte[0];
            CreateSut();

            // Act
            var encoding = sut.GetEncoding(data, dataLength: 0);

            // Assert
            Assert.Equal(EncodingType.None, encoding);
        }

        [Fact]
        public void GetEncoding_WhenDataIsNotFilledButDataLengthIsNotZero()
        {
            // Arrange
            byte[] data = GetData();
            CreateSut();

            // Act
            var encoding = sut.GetEncoding(data, dataLength: BufferSize);

            // Assert
            Assert.Equal(EncodingType.None, encoding);
        }

        [Fact]
        public void GetEncoding_WhenDataDoesNotContainBomNorEol_ThenEncodingCanNotBeDetected()
        {
            // Arrange
            byte[] data = GetData(initializeWithDummyValues: true);
            CreateSut();

            // Act
            var encoding = sut.GetEncoding(data, dataLength: BufferSize);

            // Assert
            Assert.Equal(EncodingType.None, encoding);
        }

        public static IEnumerable<object[]> BomPerUtfTestData => new List<object[]>
        {
            new object[] { new byte[3] { 0xef, 0xbb, 0xbf }, EncodingType.Utf8 },
            new object[] { new byte[2] { 0xff, 0xfe }, EncodingType.Utf16LE },
            new object[] { new byte[2] { 0xfe, 0xff }, EncodingType.Utf16BE },
            new object[] { new byte[4] { 0xff, 0xfe, EolByte.Zero, EolByte.Zero }, EncodingType.Utf32LE },
            new object[] { new byte[4] { EolByte.Zero, EolByte.Zero, 0xfe, 0xff }, EncodingType.Utf32BE },
        };

        public static IEnumerable<object[]> EolPerUtfTestData => new List<object[]>
        {
            new object[] { new byte[1] { EolByte.Cr}, EncodingType.Utf8 },
            new object[] { new byte[2] { EolByte.Cr, EolByte.Zero}, EncodingType.Utf16LE },
            new object[] { new byte[4] { EolByte.Cr, EolByte.Zero, EolByte.Zero, EolByte.Zero }, EncodingType.Utf32LE },
            new object[] { new byte[2] { EolByte.Zero, EolByte.Cr }, EncodingType.Utf16BE },
            new object[] { new byte[4] { EolByte.Zero, EolByte.Zero, EolByte.Zero, EolByte.Cr }, EncodingType.Utf32BE },

            new object[] { new byte[1] { EolByte.Lf}, EncodingType.Utf8 },
            new object[] { new byte[2] { EolByte.Lf, EolByte.Zero}, EncodingType.Utf16LE },
            new object[] { new byte[4] { EolByte.Lf, EolByte.Zero, EolByte.Zero, EolByte.Zero }, EncodingType.Utf32LE },
            new object[] { new byte[2] { EolByte.Zero, EolByte.Lf }, EncodingType.Utf16BE },
            new object[] { new byte[4] { EolByte.Zero, EolByte.Zero, EolByte.Zero, EolByte.Lf }, EncodingType.Utf32BE },

            new object[] { new byte[2] { EolByte.Cr, EolByte.Lf }, EncodingType.Utf8},
            new object[] { new byte[4] { EolByte.Cr, EolByte.Zero, EolByte.Lf, EolByte.Zero }, EncodingType.Utf16LE },
            new object[] { new byte[8] { EolByte.Cr, EolByte.Zero, EolByte.Zero, EolByte.Zero, EolByte.Lf, EolByte.Zero, EolByte.Zero, EolByte.Zero }, EncodingType.Utf32LE },
            new object[] { new byte[4] { EolByte.Zero, EolByte.Cr, EolByte.Zero, EolByte.Lf }, EncodingType.Utf16BE },
            new object[] { new byte[8] { EolByte.Zero, EolByte.Zero, EolByte.Zero, EolByte.Cr, EolByte.Zero, EolByte.Zero, EolByte.Zero, EolByte.Lf }, EncodingType.Utf32BE },
        };

        private byte[] GetData(bool initializeWithDummyValues = false)
        {
            byte[] data = new byte[BufferSize];
            if (initializeWithDummyValues)
            {
                FillDataWithDummyValues(data, BufferSize);
            }

            return data;
        }

        private void FillDataWithDummyValues(byte[] data, int dataLength)
        {
            for (int i = 0; i < dataLength; i++)
            {
                data[i] = DummyByte;
            }
        }

        private byte[] GetSurroundedByZeros(byte[] data, EncodingType encoding)
        {
            int numSurroundingZeros = encoding.GetNumBytesPerUnit() - 1;
            byte[] dataZeroSurrounded = new byte[numSurroundingZeros + data.Length];
            int copyAtIndex = encoding.IsBigEndian() ? 0 : numSurroundingZeros;
            data.CopyTo(dataZeroSurrounded, copyAtIndex);
            return dataZeroSurrounded;
        }

        private void CreateSut()
        {
            sut = new EncodingDetector();
        }
    }
}
