using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EolConverter.Encoding
{
    public interface IEncodingDetector
    {
        (EncodingType encoding, bool hasBom) GetEncoding(byte[] data, int dataLength);
    }
}
