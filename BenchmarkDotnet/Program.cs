using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using static BenchmarkDotNet.Attributes.MarkdownExporterAttribute;

namespace BenchmarkDotnet
{
    internal class Program
    {
        public static string data = "19,Winonah,Ashtonhurst,washtonhursti@people.com.cn,Female,68.161.193.249";


        static void Main(string[] args)
        {
            //var summary = BenchmarkRunner.Run<EmptyVSNewList>();
            //var summary = BenchmarkRunner.Run<Count>();
            //var summary = BenchmarkRunner.Run<ReadVSOptimizeRead>();
            //var summary = BenchmarkRunner.Run<WriteVSOptimizeWrite>();

            //WriteVSOptimizeWrite writeVSOptimizeWrite = new WriteVSOptimizeWrite();
            //writeVSOptimizeWrite.OptimizeWrite();


            // binarySearch
            //int[] ints = new int[] { 0, 1789, 2568, 3096, 4732, 5562, 7912, 7980, 9890, 12345 };
            //int target = 1;

            //Console.WriteLine(binarySearch(ints, target));
            // binarySearch


            // buildMpdIndex
            const int ROW_DATA = 7_500_000;
            const int BYTE_TARGET = 1_000_000;

            string path = @"C:\Users\Albert\Github\repos\private\c_sharp\leo_class\console\ParallelProcessingData";
            string readPath = Path.Combine(path, $@"ReadData\{ROW_DATA}_MOCK_DATA.csv");
            string mpdJsonPath = Path.Combine(path, $@"ReadData\{ROW_DATA}_MOCK_DATA.json");

            BuildIndex(readPath, mpdJsonPath, BYTE_TARGET);
            // buildMpdIndex




            //Thread.Sleep(3000);

            //Stopwatch sw = new Stopwatch();
            //sw.Start();
            //string MpdIndexString = File.ReadAllText(mpdJsonPath);
            //List<MpdIndex> mpdIndices = JsonConvert.DeserializeObject<List<MpdIndex>>(MpdIndexString);
            //sw.Stop();
            //double swRead = sw.ElapsedMilliseconds / 1000.0;
            //Console.WriteLine(swRead);

            Console.ReadKey();
        }
        public static int binarySearch(int[] ints, int target)
        {
            int left = 0;
            int right = ints.Length - 1;

            while (left <= right)
            {
                int mid = (left + right) / 2;
                if (target == ints[mid]) { return mid; }
                else if (target < ints[mid]) { right = mid - 1; }
                else if (target > ints[mid]) { left = mid + 1; }
            }

            return right;
        }
        public static void BuildIndex(string filePath, string mpdPath, int BYTE_TARGET)
        {
            List<MpdIndex> mpdIndices = new List<MpdIndex>();

            long offsets = 0;

            using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                MpdIndex mpdIndex = null;
                long currentPos = 0;
                offsets++;

                int b;
                int byteCount = 0;
                int rowCount = 1;
                int CumulativeRows = 1;
                long currentPostion = 0;
                long startPostion = 0;
                long startRows = 0;
                while ((b = fs.ReadByte()) != -1)
                {
                    currentPos++;
                    byteCount++;
                    currentPostion++;

                    if (byteCount >= BYTE_TARGET && b == 10)
                    {
                        Console.WriteLine($"currentPos: {currentPos}");
                        Console.WriteLine($"rowCount: {rowCount}");
                        Console.WriteLine($"byteCount: {byteCount}");
                        Console.WriteLine($"startPostion: {currentPostion - byteCount}");
                        Console.WriteLine($"startRows: {CumulativeRows - rowCount}");
                        startPostion = currentPostion - byteCount;
                        startRows = CumulativeRows - rowCount;
                        mpdIndex = new MpdIndex() { Rows = rowCount, StartRows = startRows, Bytes = byteCount, StartPosition = startPostion };
                        mpdIndices.Add(mpdIndex);
                        byteCount = 0;
                        rowCount = 0;
                    };
                    if (b == 10)
                    {
                        rowCount++;
                        CumulativeRows++;
                        offsets++;
                    }
                }
                Console.WriteLine($"currentPos: {currentPos}");
                Console.WriteLine($"rowCount: {rowCount}");
                Console.WriteLine($"byteCount: {byteCount}");
                Console.WriteLine($"startPostion: {currentPostion - byteCount}");
                Console.WriteLine($"startRows: {CumulativeRows - rowCount}");
                startPostion = currentPostion - byteCount;
                startRows = CumulativeRows - rowCount;
                mpdIndex = new MpdIndex() { Rows = rowCount, StartRows = startRows, Bytes = byteCount, StartPosition = startPostion };
                mpdIndices.Add(mpdIndex);
            }
            string content = JsonConvert.SerializeObject(mpdIndices);

            using (StreamWriter outputFile = new StreamWriter(mpdPath, false))
            {
                outputFile.WriteLine(content);
            }
        }
    }
}

