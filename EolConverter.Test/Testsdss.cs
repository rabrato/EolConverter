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
    public class Testsdss
    {
        const int BlockSize = 1024;

        [Fact]
        public void Test()
        {
            string filePath = @"c:\tmp\test\{0}.txt";
            string fileName = "Utf16LeBom";
            string file = String.Format(filePath, fileName);
            byte[] readBuffer = new byte[BlockSize];

            var sourceEncoding = new UnicodeEncoding(bigEndian: false, byteOrderMark: true);

            var encodingsToConvert = new Dictionary<string, System.Text.Encoding>()
            {
                ["Utf8Bom"] = new UTF8Encoding(true),
                ["Utf8NoBom"] = new UTF8Encoding(false),
                //["Utf16LeBom"] = new UnicodeEncoding(bigEndian: false, byteOrderMark: true),
                ["Utf16LeNoBom"] = new UnicodeEncoding(bigEndian: false, byteOrderMark: false),
                ["Utf16BeBom"] = new UnicodeEncoding(bigEndian: true, byteOrderMark: true),
                ["Utf16BeNoBom"] = new UnicodeEncoding(bigEndian: true, byteOrderMark: false),
                ["Utf32LeBom"] = new UTF32Encoding(bigEndian: false, byteOrderMark: true),
                ["Utf32LeNoBom"] = new UTF32Encoding(bigEndian: false, byteOrderMark: false),
                ["Utf32BeBom"] = new UTF32Encoding(bigEndian: true, byteOrderMark: true),
                ["Utf32BeNoBom"] = new UTF32Encoding(bigEndian: true, byteOrderMark: false),
            };

            foreach (var encodingToConvert in encodingsToConvert)
            {
                string outputFile = String.Format(filePath, encodingToConvert.Key);
                using (FileStream reader = new FileStream(file, FileMode.Open))
                using (FileStream readerOutput = new FileStream(outputFile, FileMode.OpenOrCreate))
                using (StreamWriter writer = new StreamWriter(readerOutput, encodingToConvert.Value))
                {
                    int read = 0;
                    while ((read = reader.Read(readBuffer, 0, BlockSize)) != 0)
                    {
                        byte[] data = readBuffer.Take(read).ToArray();
                        ConvertEncoding(data, sourceEncoding, writer);
                    }
                }

                string crOutput = String.Format(filePath, encodingToConvert.Key + "_CR");
                string lfOutput = String.Format(filePath, encodingToConvert.Key + "_LF");
                string crlfOutput = String.Format(filePath, encodingToConvert.Key + "_CRLF");
                byte[] crOutputBuffer = new byte[BlockSize * 2];
                byte[] lfOutputBuffer = new byte[BlockSize * 2];
                byte[] crlfOutputBuffer = new byte[BlockSize * 2];
                EolDataConverter crConverter = new EolDataConverter(EolConversion.CR);
                EolDataConverter lfConverter = new EolDataConverter(EolConversion.LF);
                EolDataConverter crlfConverter = new EolDataConverter(EolConversion.CRLF);

                //int i = 0;
                using (FileStream reader = new FileStream(outputFile, FileMode.Open))
                using (FileStream crWriter = new FileStream(crOutput, FileMode.Create))
                using (FileStream lfWriter = new FileStream(lfOutput, FileMode.Create))
                using (FileStream crlfWriter = new FileStream(crlfOutput, FileMode.Create))
                {
                    int read = 0;
                    int crOutputLength;
                    int lfOutputLength;
                    int crlfOutputLength;
                    while ((read = reader.Read(readBuffer, 0, BlockSize)) != 0)
                    {
                        var encoding = new EncodingDetector().GetEncoding(readBuffer, read);
                        //EolDataConverter converter = new EolDataConverter((EolConversion)(i % 3));
                        //converter.Convert(readBuffer, read, crOutputBuffer, out crOutputLength);
                        //crWriter.Write(crOutputBuffer, 0, crOutputLength);
                        //i++;

                        crConverter.Convert(readBuffer, read, crOutputBuffer, out crOutputLength);
                        crWriter.Write(crOutputBuffer, 0, crOutputLength);

                        lfConverter.Convert(readBuffer, read, lfOutputBuffer, out lfOutputLength);
                        lfWriter.Write(lfOutputBuffer, 0, lfOutputLength);

                        crlfConverter.Convert(readBuffer, read, crlfOutputBuffer, out crlfOutputLength);
                        crlfWriter.Write(crlfOutputBuffer, 0, crlfOutputLength);
                    }
                }
            }
        }

        private void ConvertEncoding<TEncoding>(byte[] data, TEncoding sourceEncoding, StreamWriter writer)
            where TEncoding : System.Text.Encoding
        {
            var dataStr = sourceEncoding.GetString(data);

            var dataInTargetEncoding = writer.Encoding.GetBytes(dataStr);
            var dataInTargetEncodingStr = writer.Encoding.GetString(dataInTargetEncoding);
            writer.Write(dataInTargetEncodingStr);
        }

        private void WriteBytes(StreamWriter writer, byte[] data, int dataLength)
        {
            foreach (var b in data.Take(dataLength))
            {
                writer.WriteLine(b.ToString());
            }
        }



        [Fact]
        public void PrintAllChars()
        {
            string fileName = "AllUTF32Chars";
            string file = $@"c:\tmp\{fileName}.txt";

            using (FileStream reader = new FileStream(file, FileMode.OpenOrCreate))
            using (StreamWriter writer = new StreamWriter(reader, new UTF32Encoding(bigEndian: false, byteOrderMark: false)))
            {
                writer.Write(@"
            Test 
            file
            ");
                writer.Close();
                reader.Close();
            }

            using (FileStream writer = new FileStream(file, FileMode.OpenOrCreate))
            {
                byte[] data;
                var nums = Enumerable.Range(0, 255).Select(n => (byte)n).ToList();
                    foreach (var num2 in nums)
                    {
                        foreach (var num3 in nums)
                        {
                            foreach (var num4 in nums)
                            {
                                data = new byte[4] { 255, num2, num3, num4 };
                                writer.Write(data, 0, data.Length);
                            }
                        }
                    }
            }
        }

        [Fact]
        public void Tes1t()
        {
            string filePath = @"c:\tmp\asa\{0}.txt";
            string fileName = "Utf16BeBom";
            string file = String.Format(filePath, fileName);
            byte[] readBuffer = new byte[BlockSize];

            int a = 0;
            string outputName = "Utf16BeNoBom";
            string outputFile = String.Format(filePath, outputName);
            using (FileStream reader = new FileStream(file, FileMode.Open))
            using (FileStream crWriter = new FileStream(outputFile, FileMode.Create))
            {
                int read = 0;
                while ((read = reader.Read(readBuffer, 0, BlockSize)) != 0)
                {
                    byte[] data = readBuffer;
                    if (a == 0)
                    {
                        int bomLength = 4;
                        data = data.Skip(bomLength).ToArray();
                        read -= bomLength;
                    }
                    crWriter.Write(data, 0, read);
                    a++;
                }
            }
            return;
            string crOutput = String.Format(filePath, outputName + "_CR");
            string lfOutput = String.Format(filePath, outputName + "_LF");
            string crlfOutput = String.Format(filePath, outputName + "_CRLF");
            byte[] crOutputBuffer = new byte[BlockSize * 2];
            byte[] lfOutputBuffer = new byte[BlockSize * 2];
            byte[] crlfOutputBuffer = new byte[BlockSize * 2];
            EolDataConverter crConverter = new EolDataConverter(EolConversion.CR);
            EolDataConverter lfConverter = new EolDataConverter(EolConversion.LF);
            EolDataConverter crlfConverter = new EolDataConverter(EolConversion.CRLF);

            //int i = 0;
            using (FileStream reader = new FileStream(outputFile, FileMode.Open))
            using (FileStream crWriter = new FileStream(crOutput, FileMode.Create))
            using (FileStream lfWriter = new FileStream(lfOutput, FileMode.Create))
            using (FileStream crlfWriter = new FileStream(crlfOutput, FileMode.Create))
            {
                int read = 0;
                int crOutputLength;
                int lfOutputLength;
                int crlfOutputLength;
                while ((read = reader.Read(readBuffer, 0, BlockSize)) != 0)
                {
                    var encoding = new EncodingDetector().GetEncoding(readBuffer, read);
                    //EolDataConverter converter = new EolDataConverter((EolConversion)(i % 3));
                    //converter.Convert(readBuffer, read, crOutputBuffer, out crOutputLength);
                    //crWriter.Write(crOutputBuffer, 0, crOutputLength);
                    //i++;

                    crConverter.Convert(readBuffer, read, crOutputBuffer, out crOutputLength);
                    crWriter.Write(crOutputBuffer, 0, crOutputLength);

                    lfConverter.Convert(readBuffer, read, lfOutputBuffer, out lfOutputLength);
                    lfWriter.Write(lfOutputBuffer, 0, lfOutputLength);

                    crlfConverter.Convert(readBuffer, read, crlfOutputBuffer, out crlfOutputLength);
                    crlfWriter.Write(crlfOutputBuffer, 0, crlfOutputLength);
                }
            }
        }




        [Fact]
        public void sdfsdfdsfds()
        {
            string filePath = @"c:\tmp\asa\{0}.txt";
            byte[] readBuffer = new byte[BlockSize];

            var filesToCheck = new Dictionary<string, (EncodingType encoding, bool hasBom)>()
            {
                ["Utf8Bom"] = (EncodingType.Utf8, true),
                ["Utf8NoBom"] = (EncodingType.Utf8, false),
                ["Utf16LeBom"] = (EncodingType.Utf16LE, true),
                ["Utf16LeNoBom"] = (EncodingType.Utf16LE, false),
                ["Utf16BeBom"] = (EncodingType.Utf16BE, true),
                ["Utf16BeNoBom"] = (EncodingType.Utf16BE, false),
                ["Utf32LeBom"] = (EncodingType.Utf32LE, true),
                ["Utf32LeNoBom"] = (EncodingType.Utf32LE, false),
                ["Utf32BeBom"] = (EncodingType.Utf32BE, true),
                ["Utf32BeNoBom"] = (EncodingType.Utf32BE, false),
            };

            foreach (var item in filesToCheck)
            {
                using (FileStream reader = new FileStream(String.Format(filePath, item.Key), FileMode.Open))
                {
                    int read = 0;
                    if ((read = reader.Read(readBuffer, 0, BlockSize)) != 0)
                    {
                        var encoding = new EncodingDetector().GetEncoding(readBuffer, read);
                        Assert.Equal(item.Value.encoding, encoding.encoding);
                        Assert.Equal(item.Value.hasBom, encoding.hasBom);
                    }
                }
            }
        }

        [Fact]
        public void sdfsdfdsf4ds()
        {
            string filePath = @"c:\tmp\asa\{0}.txt";
            byte[] readBuffer = new byte[BlockSize];

            var filesToCheck = new Dictionary<string, (EncodingType encoding, bool hasBom)>()
            {
                ["Utf8Bom"] = (EncodingType.Utf8, true),
                ["Utf8NoBom"] = (EncodingType.Utf8, false),
                ["Utf16LeBom"] = (EncodingType.Utf16LE, true),
                ["Utf16LeNoBom"] = (EncodingType.Utf16LE, false),
                ["Utf16BeBom"] = (EncodingType.Utf16BE, true),
                ["Utf16BeNoBom"] = (EncodingType.Utf16BE, false),
                ["Utf32LeBom"] = (EncodingType.Utf32LE, true),
                ["Utf32LeNoBom"] = (EncodingType.Utf32LE, false),
                ["Utf32BeBom"] = (EncodingType.Utf32BE, true),
                ["Utf32BeNoBom"] = (EncodingType.Utf32BE, false),
            };
            foreach (var item in filesToCheck)
            {
                string outputName = item.Key;
                string crOutput = String.Format(filePath, outputName + "_CR");
                string lfOutput = String.Format(filePath, outputName + "_LF");
                string crlfOutput = String.Format(filePath, outputName + "_CRLF");
                byte[] crOutputBuffer = new byte[BlockSize * 2];
                byte[] lfOutputBuffer = new byte[BlockSize * 2];
                byte[] crlfOutputBuffer = new byte[BlockSize * 2];
                EolDataConverter crConverter = new EolDataConverter(EolConversion.CR);
                EolDataConverter lfConverter = new EolDataConverter(EolConversion.LF);
                EolDataConverter crlfConverter = new EolDataConverter(EolConversion.CRLF);

                //int i = 0;
                using (FileStream reader = new FileStream(String.Format(filePath, outputName), FileMode.Open))
                using (FileStream crWriter = new FileStream(crOutput, FileMode.Create))
                using (FileStream lfWriter = new FileStream(lfOutput, FileMode.Create))
                using (FileStream crlfWriter = new FileStream(crlfOutput, FileMode.Create))
                {
                    int read = 0;
                    int crOutputLength;
                    int lfOutputLength;
                    int crlfOutputLength;
                    while ((read = reader.Read(readBuffer, 0, BlockSize)) != 0)
                    {
                        crConverter.Convert(readBuffer, read, crOutputBuffer, out crOutputLength);
                        crWriter.Write(crOutputBuffer, 0, crOutputLength);

                        lfConverter.Convert(readBuffer, read, lfOutputBuffer, out lfOutputLength);
                        lfWriter.Write(lfOutputBuffer, 0, lfOutputLength);

                        crlfConverter.Convert(readBuffer, read, crlfOutputBuffer, out crlfOutputLength);
                        crlfWriter.Write(crlfOutputBuffer, 0, crlfOutputLength);
                    }
                }
            }
        }
    }
}
