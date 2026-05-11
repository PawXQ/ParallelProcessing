using CSVLibrary;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ParallelProcessing
{
    internal class Program
    {
        private static readonly SemaphoreSlim CsvWriteLock = new SemaphoreSlim(1, 1);

        static async Task Main(string[] args)
        {
            const int BATCH_QUANTITY = 2_500_000;
            const int ROW_DATA = 9_000_000;
            const int BATCH = ROW_DATA % BATCH_QUANTITY == 0 ? ROW_DATA / BATCH_QUANTITY : ROW_DATA / BATCH_QUANTITY + 1;

            string path = @"C:\Users\Albert\Github\repos\private\c_sharp\leo_class\console\ParallelProcessingData";
            string readPath = Path.Combine(path, $@"ReadData\{ROW_DATA}_MOCK_DATA.csv");
            string writePath = Path.Combine(path, $@"WriteData\{ROW_DATA}_MOCK_DATA.csv");

            // MpdIndex
            string mpdJsonPath = Path.Combine(path, $@"ReadData\{ROW_DATA}_MOCK_DATA.json");
            string MpdIndexString = File.ReadAllText(mpdJsonPath);
            List<MpdIndex> mpdIndices = JsonConvert.DeserializeObject<List<MpdIndex>>(MpdIndexString);
            List<long> mpdIndicesStartRows = mpdIndices.Select(x => x.StartRows).ToList();
            // MpdIndex

            if (File.Exists(writePath))
                File.Delete(writePath);


            List<double> readTimes = new List<double>();
            List<double> writeTimes = new List<double>();

            List<Task> tasks = new List<Task>();

            Stopwatch swTotal = new Stopwatch();
            swTotal.Start();

            for (int i = 0; i < BATCH; i++)
            {
                int index = i;
                Stopwatch sw = new Stopwatch();

                Task taskRW = Task.Run(async () =>
                {
                    int start = index * BATCH_QUANTITY + 1;

                    // MpdIndex
                    int MpdIndex = binarySearch(mpdIndicesStartRows, start);
                    Console.WriteLine($"Batch{index + 1} MpdIndex: {MpdIndex}");
                    long MpdStartPosition = mpdIndices[MpdIndex].StartPosition;
                    long MpdStartRow = mpdIndices[MpdIndex].StartRows;
                    // MpdIndex

                    sw.Start();
                    //List<Record> record_list = CSVHelper.Read<Record>(readPath, start, BATCH_QUANTITY);

                    // MpdIndex
                    List<Record> record_list = CSVHelper.ReadMpd<Record>(readPath, MpdStartPosition, MpdStartRow, start, BATCH_QUANTITY);
                    // MpdIndex

                    sw.Stop();
                    double swRead = sw.ElapsedMilliseconds / 1000.0;
                    readTimes.Add(swRead);
                    Console.WriteLine($"Batch{index + 1} read: {swRead}");

                    sw.Restart();

                    await CsvWriteLock.WaitAsync();
                    try
                    {
                        CSVHelper.WriteList(writePath, record_list, true);
                    }
                    finally
                    {
                        CsvWriteLock.Release();
                    }

                    sw.Stop();
                    double swWrite = sw.ElapsedMilliseconds / 1000.0;
                    writeTimes.Add(swWrite);
                    Console.WriteLine($"Batch{index + 1} write: {swWrite}");

                    Console.WriteLine($"Batch{index + 1} total: {swRead + swWrite}");
                });

                tasks.Add(taskRW);
            }

            await Task.WhenAll(tasks);

            swTotal.Stop();
            Console.WriteLine(swTotal.ElapsedMilliseconds / 1000.0);

            double readMedian = readTimes.Median(x => x);
            double writedMedian = writeTimes.Median(x => x);

            Console.WriteLine($"ReadMediam: {readMedian}");
            Console.WriteLine($"WriteMediam: {writedMedian}");

            Console.WriteLine($"|  {BATCH_QUANTITY.ToString("#,##0")}     | {ROW_DATA.ToString("#,##0")}     |{Math.Round(readTimes.Median(x => x), 2)}            |     {Math.Round(writeTimes.Median(x => x), 2)}            |     {Math.Round(swTotal.ElapsedMilliseconds / 1000.0, 2)}          |                |");

            Console.ReadKey();
        }

        public static int binarySearch(List<long> ints, int target)
        {
            int left = 0;
            int right = ints.Count - 1;

            while (left <= right)
            {
                int mid = (left + right) / 2;
                if (target == ints[mid]) { return mid; }
                else if (target < ints[mid]) { right = mid - 1; }
                else if (target > ints[mid]) { left = mid + 1; }
            }

            return right;
        }
    }
}
