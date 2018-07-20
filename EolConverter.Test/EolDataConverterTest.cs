using EolConverter.Encoding;
using EolConverter.Test.TestUtils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace EolConverter.Test
{
    public class EolDataConverterTest
    {
        [Fact]
        public void Test()
        {
            string file = @"c:\tmp\UTF16BENoBOM.txt";
            string fileOutput = @"c:\tmp\fileoutput_32.c";
            const int BlockSize = 1024;

            byte[] readBuffer = new byte[BlockSize];
            byte[] outputBuffer = new byte[BlockSize * 2];
            EolDataConverter eolConverter = new EolDataConverter(EolConversion.CR);
            
//            using (FileStream reader = new FileStream(file, FileMode.OpenOrCreate))
//            using (StreamWriter writer = new StreamWriter(reader, new System.Text.UTF8Encoding(false)))
//            {
//                writer.Write(@"
//Test 
//file
//");
//                writer.Close();
//                reader.Close();
//            }

            using (FileStream reader = new FileStream(file, FileMode.Open))
            using (FileStream writer = new FileStream(fileOutput, FileMode.Create))
            {
                int read = 0;
                int outputLength;
                while ((read = reader.Read(readBuffer, 0, BlockSize)) != 0)
                {
                    var encoding = new EncodingDetector().GetEncoding(readBuffer, read);
                    eolConverter.Convert(readBuffer, read, outputBuffer, out outputLength);
                    writer.Write(outputBuffer, 0, outputLength);
                }
            }
        }
    }
}
