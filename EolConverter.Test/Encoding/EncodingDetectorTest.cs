﻿using EolConverter.ByteUtils;
using EolConverter.Encoding;
using System.Collections.Generic;
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
            byte[] data = new byte[10];
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
            eolBytes.CopyTo(data, data.Length / 2);

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
            eolBytes.CopyTo(data, data.Length - eolBytes.Length);

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
            int bufferLength = 10 * expectedEncoding.GetNumBytesPerUnit();
            byte[] data = new byte[bufferLength];
            int dataLength = bufferLength / 2;
            FillDataWithDummyValues(data, dataLength);
            eolBytes.CopyTo(data, dataLength - eolBytes.Length);

            CreateSut();

            // Act
            var encoding = sut.GetEncoding(data, dataLength).encoding;

            // Assert
            Assert.Equal(expectedEncoding, encoding);
        }

        [Theory]
        [InlineData(new byte[2] { ByteCode.CR, 0, }, 11)]
        [InlineData(new byte[4] { ByteCode.CR, 0, 0, 0 }, 9)]
        [InlineData(new byte[4] { ByteCode.LF, 0, 0, 0 }, 10)]
        [InlineData(new byte[4] { ByteCode.LF, 0, 0, 0 }, 11)]
        public void GetEncoding_WhenEolIsNotFirstByteInUnitAndIsFollowedByNullBytes_ThenEncodingCanNotBeDetected(byte[] eolBytesFollowedByNullBytes, int startIndex)
        {
            // Arrange
            byte[] data = GetData();
            eolBytesFollowedByNullBytes.CopyTo(data, startIndex);

            CreateSut();

            // Act
            var encoding = sut.GetEncoding(data, data.Length).encoding;

            // Assert
            Assert.Equal(EncodingType.None, encoding);
        }

        [Theory]
        [InlineData(new byte[2] { 0, ByteCode.CR }, 9)]
        [InlineData(new byte[4] { 0, 0, 0, ByteCode.CR }, 5)]
        [InlineData(new byte[4] { 0, 0, 0, ByteCode.LF }, 6)]
        [InlineData(new byte[4] { 0, 0, 0, ByteCode.LF }, 7)]
        public void GetEncoding_WhenEolIsNotLastByteInUnitAndIsPrecededByNullBytes_ThenEncodingCanNotBeDetected(byte[] eolBytesPrecededByNullBytes, int startIndex)
        {
            // Arrange
            byte[] data = GetData();
            eolBytesPrecededByNullBytes.CopyTo(data, startIndex);

            CreateSut();

            // Act
            var encoding = sut.GetEncoding(data, data.Length).encoding;

            // Assert
            Assert.Equal(EncodingType.None, encoding);
        }

        [Fact]
        public void GetEncoding_WhenDataContainsOneByteEolButAlsoANullByte_ThenEncoding()
        {
            // Arrange
            byte[] data = new byte[5] { 1, 0, 1, ByteCode.CR, 1 };

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
            byte[] data = new byte[100];
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
            byte[] data = new byte[100];
            CreateSut();

            // Act
            var encoding = sut.GetEncoding(data, dataLength: 10).encoding;

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
            new object[] { new byte[4] { 0xff, 0xfe, ByteCode.Null, ByteCode.Null }, EncodingType.Utf32LE },
            new object[] { new byte[4] { ByteCode.Null, ByteCode.Null, 0xfe, 0xff }, EncodingType.Utf32BE },
        };

        public static IEnumerable<object[]> EolPerUtfTestData => new List<object[]>
        {
            new object[] { new byte[1] { ByteCode.CR}, EncodingType.Utf8 },
            new object[] { new byte[2] { ByteCode.CR, ByteCode.Null}, EncodingType.Utf16LE },
            new object[] { new byte[4] { ByteCode.CR, ByteCode.Null, ByteCode.Null, ByteCode.Null }, EncodingType.Utf32LE },
            new object[] { new byte[2] { ByteCode.Null, ByteCode.CR }, EncodingType.Utf16BE },
            new object[] { new byte[4] { ByteCode.Null, ByteCode.Null, ByteCode.Null, ByteCode.CR }, EncodingType.Utf32BE },

            new object[] { new byte[1] { ByteCode.LF}, EncodingType.Utf8 },
            new object[] { new byte[2] { ByteCode.LF, ByteCode.Null}, EncodingType.Utf16LE },
            new object[] { new byte[4] { ByteCode.LF, ByteCode.Null, ByteCode.Null, ByteCode.Null }, EncodingType.Utf32LE },
            new object[] { new byte[2] { ByteCode.Null, ByteCode.LF }, EncodingType.Utf16BE },
            new object[] { new byte[4] { ByteCode.Null, ByteCode.Null, ByteCode.Null, ByteCode.LF }, EncodingType.Utf32BE },

            new object[] { new byte[2] { ByteCode.CR, ByteCode.LF }, EncodingType.Utf8},
            new object[] { new byte[4] { ByteCode.CR, ByteCode.Null, ByteCode.LF, ByteCode.Null }, EncodingType.Utf16LE },
            new object[] { new byte[8] { ByteCode.CR, ByteCode.Null, ByteCode.Null, ByteCode.Null, ByteCode.LF, ByteCode.Null, ByteCode.Null, ByteCode.Null }, EncodingType.Utf32LE },
            new object[] { new byte[4] { ByteCode.Null, ByteCode.CR, ByteCode.Null, ByteCode.LF }, EncodingType.Utf16BE },
            new object[] { new byte[8] { ByteCode.Null, ByteCode.Null, ByteCode.Null, ByteCode.CR, ByteCode.Null, ByteCode.Null, ByteCode.Null, ByteCode.LF }, EncodingType.Utf32BE },
        };

        private byte[] GetData()
        {
            byte[] data = new byte[BufferSize];
            FillDataWithDummyValues(data);
            return data;
        }

        private void FillDataWithDummyValues(byte[] data, int? dataLength = null)
        {
            dataLength = dataLength ?? data.Length;
            for (int i = 0; i < dataLength; i++)
            {
                data[i] = DummyByte;
            }
        }

        private void CreateSut()
        {
            sut = new EncodingDetector();
        }
    }
}
