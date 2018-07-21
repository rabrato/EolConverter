using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EolConverter.Test.TestUtils
{
    internal static class DummyByteDataProvider
    {
        private const byte DummyByte = 100;
        internal const int BufferSize = 1024;

        internal static byte[] GetDummyData()
        {
            byte[] data = new byte[BufferSize];
            data.FillDataWithDummyValues();
            return data;
        }

        internal static void FillDataWithDummyValues(this byte[] data, int? dataLength = null)
        {
            dataLength = dataLength ?? data.Length;
            for (int i = 0; i < dataLength; i++)
            {
                data[i] = DummyByte;
            }
        }
    }
}
