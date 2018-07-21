using EolConverter.Encoding;
using EolConverter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using EolConverter.ByteUtils;

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
            (var encoding, bool hasBom) = sut.GetEncoding(data, data.Length);

            // Assert
            Assert.True(hasBom);
            Assert.Equal(expectedEncoding, encoding);
        }

        [Fact]
        public void GetEncoding_WhenDataHasNotBom()
        {
            // Arrange
            byte[] data = GetData();

            CreateSut();

            // Act
            bool hasBom = sut.GetEncoding(data, data.Length).hasBom;

            // Assert
            Assert.False(hasBom);
        }

        [Theory, MemberData(nameof(EolPerUtfTestData))]
        public void GetEncoding_WhenDataOnlyContainsEol(byte[] eolBytes, EncodingType expectedEncoding)
        {
            // Arrange
            byte[] data = GetData(initializeWithDummyValues: false);
            eolBytes.CopyTo(data, 0);

            CreateSut();

            // Act
            var encoding = sut.GetEncoding(data, dataLength: eolBytes.Length).encoding;

            // Assert
            Assert.Equal(expectedEncoding, encoding);
        }

        [Theory, MemberData(nameof(EolPerUtfTestData))]
        public void GetEncoding_WhenDataStartsByEol(byte[] eolBytes, EncodingType expectedEncoding)
        {
            // Arrange
            byte[] data = GetData();
            eolBytes.CopyTo(data, 0);

            CreateSut();

            // Act
            var encoding = sut.GetEncoding(data, data.Length).encoding;

            // Assert
            Assert.Equal(expectedEncoding, encoding);
        }

        [Theory, MemberData(nameof(EolPerUtfTestData))]
        public void GetEncoding_WhenDataContainsEol(byte[] eolBytes, EncodingType expectedEncoding)
        {
            // Arrange
            byte[] data = GetData();
            eolBytes.CopyTo(data, BufferSize / 2);

            CreateSut();

            // Act
            var encoding = sut.GetEncoding(data, data.Length).encoding;

            // Assert
            Assert.Equal(expectedEncoding, encoding);
        }

        [Theory, MemberData(nameof(EolPerUtfTestData))]
        public void GetEncoding_WhenDataEndsByEol(byte[] eolBytes, EncodingType expectedEncoding)
        {
            // Arrange
            byte[] data = GetData();
            eolBytes.CopyTo(data, BufferSize - eolBytes.Length);

            CreateSut();

            // Act
            var encoding = sut.GetEncoding(data, data.Length).encoding;

            // Assert
            Assert.Equal(expectedEncoding, encoding);
        }

        [Theory, MemberData(nameof(EolPerUtfTestData))]
        public void GetEncoding_WhenDataEndsByEolButDataIsNotCompletelyFilled(byte[] eolBytes, EncodingType expectedEncoding)
        {
            // Arrange
            byte[] data = GetData(initializeWithDummyValues: false);
            int dataLength = 10 * expectedEncoding.GetNumBytesPerUnit();
            FillDataWithDummyValues(data, dataLength);
            eolBytes.CopyTo(data, dataLength - eolBytes.Length);

            CreateSut();

            // Act
            var encoding = sut.GetEncoding(data, dataLength).encoding;

            // Assert
            Assert.Equal(expectedEncoding, encoding);
        }

        [Theory]
        [InlineData(new byte[2] { ByteCode.Cr, 0, }, 11)]
        [InlineData(new byte[4] { ByteCode.Cr, 0, 0, 0 }, 9)]
        [InlineData(new byte[4] { ByteCode.Lf, 0, 0, 0 }, 10)]
        [InlineData(new byte[4] { ByteCode.Lf, 0, 0, 0 }, 11)]
        public void GetEncoding_WhenEolIsNotFirstByteInUnitAndIsFollowedByZeros_ThenEncodingCanNotBeDetected(byte[] eolBytesFollowedByZeros, int startIndex)
        {
            // Arrange
            byte[] data = GetData();
            eolBytesFollowedByZeros.CopyTo(data, startIndex);

            CreateSut();

            // Act
            var encoding = sut.GetEncoding(data, data.Length).encoding;

            // Assert
            Assert.Equal(EncodingType.None, encoding);
        }

        [Theory]
        [InlineData(new byte[2] { 0, ByteCode.Cr }, 9)]
        [InlineData(new byte[4] { 0, 0, 0, ByteCode.Cr }, 5)]
        [InlineData(new byte[4] { 0, 0, 0, ByteCode.Lf }, 6)]
        [InlineData(new byte[4] { 0, 0, 0, ByteCode.Lf }, 7)]
        public void GetEncoding_WhenEolIsNotLastByteInUnitAndIsPrecededByZeros_ThenEncodingCanNotBeDetected(byte[] eolBytesPrecededByZeros, int startIndex)
        {
            // Arrange
            byte[] data = GetData();
            eolBytesPrecededByZeros.CopyTo(data, startIndex);

            CreateSut();

            // Act
            var encoding = sut.GetEncoding(data, data.Length).encoding;

            // Assert
            Assert.Equal(EncodingType.None, encoding);
        }

        [Fact]
        public void GetEncoding_WhenDataIsNull()
        {
            // Arrange
            CreateSut();

            // Act
            var encoding = sut.GetEncoding(data: null, dataLength: 0).encoding;

            // Assert
            Assert.Equal(EncodingType.None, encoding);
        }

        [Fact]
        public void GetEncoding_WhenDataLengthIsZero()
        {
            // Arrange
            byte[] data = GetData(initializeWithDummyValues: false);
            CreateSut();

            // Act
            var encoding = sut.GetEncoding(data, dataLength: 0).encoding;

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
            var encoding = sut.GetEncoding(data, dataLength: 0).encoding;

            // Assert
            Assert.Equal(EncodingType.None, encoding);
        }

        [Fact]
        public void GetEncoding_WhenDataIsNotFilledButDataLengthIsNotZero()
        {
            // Arrange
            byte[] data = GetData(initializeWithDummyValues: false);
            CreateSut();

            // Act
            var encoding = sut.GetEncoding(data, dataLength: BufferSize).encoding;

            // Assert
            Assert.Equal(EncodingType.None, encoding);
        }

        [Fact]
        public void GetEncoding_WhenDataDoesNotContainBomNorEol_ThenEncodingCanNotBeDetected()
        {
            // Arrange
            byte[] data = GetData();
            CreateSut();

            // Act
            var encoding = sut.GetEncoding(data, data.Length).encoding;

            // Assert
            Assert.Equal(EncodingType.None, encoding);
        }

        public static IEnumerable<object[]> BomPerUtfTestData => new List<object[]>
        {
            new object[] { new byte[3] { 0xef, 0xbb, 0xbf }, EncodingType.Utf8 },
            new object[] { new byte[2] { 0xff, 0xfe }, EncodingType.Utf16LE },
            new object[] { new byte[2] { 0xfe, 0xff }, EncodingType.Utf16BE },
            new object[] { new byte[4] { 0xff, 0xfe, ByteCode.Empty, ByteCode.Empty }, EncodingType.Utf32LE },
            new object[] { new byte[4] { ByteCode.Empty, ByteCode.Empty, 0xfe, 0xff }, EncodingType.Utf32BE },
        };

        public static IEnumerable<object[]> EolPerUtfTestData => new List<object[]>
        {
            new object[] { new byte[1] { ByteCode.Cr}, EncodingType.Utf8 },
            new object[] { new byte[2] { ByteCode.Cr, ByteCode.Empty}, EncodingType.Utf16LE },
            new object[] { new byte[4] { ByteCode.Cr, ByteCode.Empty, ByteCode.Empty, ByteCode.Empty }, EncodingType.Utf32LE },
            new object[] { new byte[2] { ByteCode.Empty, ByteCode.Cr }, EncodingType.Utf16BE },
            new object[] { new byte[4] { ByteCode.Empty, ByteCode.Empty, ByteCode.Empty, ByteCode.Cr }, EncodingType.Utf32BE },

            new object[] { new byte[1] { ByteCode.Lf}, EncodingType.Utf8 },
            new object[] { new byte[2] { ByteCode.Lf, ByteCode.Empty}, EncodingType.Utf16LE },
            new object[] { new byte[4] { ByteCode.Lf, ByteCode.Empty, ByteCode.Empty, ByteCode.Empty }, EncodingType.Utf32LE },
            new object[] { new byte[2] { ByteCode.Empty, ByteCode.Lf }, EncodingType.Utf16BE },
            new object[] { new byte[4] { ByteCode.Empty, ByteCode.Empty, ByteCode.Empty, ByteCode.Lf }, EncodingType.Utf32BE },

            new object[] { new byte[2] { ByteCode.Cr, ByteCode.Lf }, EncodingType.Utf8},
            new object[] { new byte[4] { ByteCode.Cr, ByteCode.Empty, ByteCode.Lf, ByteCode.Empty }, EncodingType.Utf16LE },
            new object[] { new byte[8] { ByteCode.Cr, ByteCode.Empty, ByteCode.Empty, ByteCode.Empty, ByteCode.Lf, ByteCode.Empty, ByteCode.Empty, ByteCode.Empty }, EncodingType.Utf32LE },
            new object[] { new byte[4] { ByteCode.Empty, ByteCode.Cr, ByteCode.Empty, ByteCode.Lf }, EncodingType.Utf16BE },
            new object[] { new byte[8] { ByteCode.Empty, ByteCode.Empty, ByteCode.Empty, ByteCode.Cr, ByteCode.Empty, ByteCode.Empty, ByteCode.Empty, ByteCode.Lf }, EncodingType.Utf32BE },
        };

        private byte[] GetData(bool initializeWithDummyValues = true)
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
