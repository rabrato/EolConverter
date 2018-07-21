using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EolConverter.Encoding;
using Moq;

namespace EolConverter.Test.TestUtils
{
    public static class IEncodingDetectorMockBuilder
    {
        public static Mock<IEncodingDetector> Create()
        {
            return new Mock<IEncodingDetector>();
        }

        public static Mock<IEncodingDetector> WithDetectedEncoding(this Mock<IEncodingDetector> mock, EncodingType returnedEncoding, bool returnedHasBom)
        {
            mock.Setup(m => m.GetEncoding(It.IsAny<byte[]>(), It.IsAny<int>())).Returns((returnedEncoding, returnedHasBom));
            return mock;
        }
    }
}
