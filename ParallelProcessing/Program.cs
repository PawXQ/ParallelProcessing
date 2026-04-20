using CSVLibrary;
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
        static async Task Main(string[] args)
        {
            object key = new object();

            const int BATCH_QUANTITY = 2_500_000;
            const int ROW_DATA = 13_000_000;
            const int BATCH = ROW_DATA % BATCH_QUANTITY == 0 ? ROW_DATA / BATCH_QUANTITY : ROW_DATA / BATCH_QUANTITY + 1;

            string path = @"C:\Users\Albert\Github\repos\private\c_sharp\leo_class\console\ParallelProcessingData";
            string readPath = Path.Combine(path, $@"ReadData\{ROW_DATA}_MOCK_DATA.csv");
            string writePath = Path.Combine(path, $@"WriteData\{ROW_DATA}_MOCK_DATA.csv");

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

                Task taskRW = Task.Run(() =>
                {
                    int start = index * BATCH_QUANTITY + 1;

                    sw.Start();
                    List<Record> record_list = CSVHelper.Read<Record>(readPath, start, BATCH_QUANTITY);
                    sw.Stop();
                    double swRead = sw.ElapsedMilliseconds / 1000.0;
                    readTimes.Add(swRead);
                    Console.WriteLine($"Batch{index + 1} read: {swRead}");

                    sw.Restart();
                    lock (key)
                    {
                        CSVHelper.WriteList(writePath, record_list, true);
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
    }
}
