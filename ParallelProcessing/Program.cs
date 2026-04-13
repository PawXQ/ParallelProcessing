using CSVLibrary;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ParallelProcessing
{
    internal class Program
    {
        static void Main(string[] args)
        {
            const int BATCH_QUANTITY = 2_500_000;
            const int ROW_DATA = 10_000_000;
            const int BATCH = ROW_DATA % BATCH_QUANTITY == 0 ? ROW_DATA / BATCH_QUANTITY : ROW_DATA / BATCH_QUANTITY + 1;

            string path = @"C:\Users\Albert\Github\repos\private\c_sharp\leo_class\console\ParallelProcessingData";
            string readPath = Path.Combine(path, $@"ReadData\{ROW_DATA}_MOCK_DATA.csv");
            string writePath = Path.Combine(path, $@"WriteData\{ROW_DATA}_MOCK_DATA.csv");

            if (File.Exists(writePath))
                File.Delete(writePath);


            //Stopwatch sw = new Stopwatch();
            //sw.Start();

            //List<Record> record_list = CSVHelper.Read<Record>(readPath);

            //sw.Stop();
            //double swRead = sw.ElapsedMilliseconds / 1000.0;
            //Console.WriteLine(swRead);

            //sw.Restart();

            //CSVHelper.WriteList(writePath, record_list, true);

            //double swWrite = sw.ElapsedMilliseconds / 1000.0;
            //Console.WriteLine(swWrite);

            //Console.WriteLine(swRead + swWrite);

            List<double> readTimes = new List<double>();
            List<double> writeTimes = new List<double>();

            Stopwatch swTotal = new Stopwatch();
            swTotal.Start();

            for (int i = 0; i < BATCH; i++)
            {
                Stopwatch sw = new Stopwatch();
                int start = i * BATCH_QUANTITY + 1;

                sw.Start();
                List<Record> record_list = CSVHelper.Read<Record>(readPath, start, BATCH_QUANTITY);
                sw.Stop();
                double swRead = sw.ElapsedMilliseconds / 1000.0;
                readTimes.Add(swRead);
                Console.WriteLine($"Batch{i + 1} read: {swRead}");

                sw.Restart();
                CSVHelper.WriteList(writePath, record_list, true);
                sw.Stop();
                double swWrite = sw.ElapsedMilliseconds / 1000.0;
                writeTimes.Add(swWrite);
                Console.WriteLine($"Batch{i + 1} write: {swWrite}");

                Console.WriteLine($"Batch{i + 1} total: {swRead + swWrite}");
            }

            swTotal.Stop();
            Console.WriteLine(swTotal.ElapsedMilliseconds / 1000.0);

            double readMedian = readTimes.Median(x => x);
            double writedMedian = writeTimes.Median(x => x);

            Console.WriteLine($"ReadMediam: {readMedian}");
            Console.WriteLine($"WriteMediam: {writedMedian}");

            Console.WriteLine($"|  {BATCH_QUANTITY.ToString("#,##0")}     | {ROW_DATA.ToString("#,##0")}     |{Math.Round(readTimes.Median(x => x), 2)}            |     {Math.Round(writeTimes.Median(x => x), 2)}            |     {Math.Round(swTotal.ElapsedMilliseconds / 1000.0, 2)}          |                |");
            //Console.WriteLine($"|  {batchcount.ToString("#,##0")}     | {ROW_COUNT.ToString("#,##0")}     |{Math.Round(EnumerableExtension.Median(readTimes), 2)}            |     {Math.Round(EnumerableExtension.Median(writeTimes), 2)}            |     {Math.Round(totalStopwatch.ElapsedMilliseconds / 1000.0, 2)}          |                |");



            Console.ReadKey();
        }
    }
}
