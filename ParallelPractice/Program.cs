// See https://aka.ms/new-console-template for more information

using CSVLibrary;
using Newtonsoft.Json;
using ParallelPractice;
using System.Diagnostics;
using System.IO.MemoryMappedFiles;

object obj = new object();

const int BATCH_QUANTITY = 2_500_000;
const int ROW_DATA = 30_000_000;
//const int ROW_DATA = 10;
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

await Parallel.ForAsync(0, BATCH, (number, token) =>
{
    int index = number;
    Stopwatch sw = new Stopwatch();

    int start = index * BATCH_QUANTITY + 1;

    //// MpdIndex
    //int MpdIndex = binarySearch(mpdIndicesStartRows, start);
    //Console.WriteLine($"Batch{index + 1} MpdIndex: {MpdIndex}");
    //long MpdStartPosition = mpdIndices[MpdIndex].StartPosition;
    //long MpdStartRow = mpdIndices[MpdIndex].StartRows;
    //// MpdIndex

    sw.Start();
    List<Record> record_list = CSVHelper.Read<Record>(readPath, start, BATCH_QUANTITY);
    //List<Record> record_list = CSVHelper.OptimizeRead<Record>(readPath, start, BATCH_QUANTITY);

    // MpdIndex
    //List<Record> record_list = CSVHelper.ReadMpd<Record>(readPath, MpdStartPosition, MpdStartRow, start, BATCH_QUANTITY);
    //List<Record> record_list = CSVHelper.OptimizeReadMpd<Record>(readPath, MpdStartPosition, MpdStartRow, start, BATCH_QUANTITY);

    // MpdIndex

    sw.Stop();
    double swRead = sw.ElapsedMilliseconds / 1000.0;
    readTimes.Add(swRead);
    Console.WriteLine($"Batch{index + 1} read: {swRead}");

    sw.Restart();
    lock (obj)
    {
        //CSVHelper.WriteList(writePath, record_list, true);
        CSVHelper.OptimizeWriteList(writePath, record_list, true);
    }

    sw.Stop();
    double swWrite = sw.ElapsedMilliseconds / 1000.0;
    writeTimes.Add(swWrite);
    Console.WriteLine($"Batch{index + 1} write: {swWrite}");

    Console.WriteLine($"Batch{index + 1} total: {swRead + swWrite}");

    return ValueTask.CompletedTask;
});

swTotal.Stop();
Console.WriteLine(swTotal.ElapsedMilliseconds / 1000.0);

double readMedian = readTimes.Median(x => x);
double writedMedian = writeTimes.Median(x => x);

Console.WriteLine($"ReadMediam: {readMedian}");
Console.WriteLine($"WriteMediam: {writedMedian}");

Console.WriteLine($"|  {BATCH_QUANTITY.ToString("#,##0")}     | {ROW_DATA.ToString("#,##0")}     |{Math.Round(readTimes.Median(x => x), 2)}            |     {Math.Round(writeTimes.Median(x => x), 2)}            |     {Math.Round(swTotal.ElapsedMilliseconds / 1000.0, 2)}          |                |");

Console.ReadKey();

//Console.WriteLine($"Thread_ID: {Thread.CurrentThread.ManagedThreadId} , Number:{0}");

//Task.Run(() =>
//{
//    Console.WriteLine($"Thread_ID: {Thread.CurrentThread.ManagedThreadId} , Number:{1}");
//});


//Task.Run(() =>
//{
//    Console.WriteLine($"Thread_ID: {Thread.CurrentThread.ManagedThreadId} , Number:{2}");
//});

//Task.Run(() =>
//{
//    Console.WriteLine($"Thread_ID: {Thread.CurrentThread.ManagedThreadId} , Number:{3}");
//});


//Task.Run(() =>
//{
//    Console.WriteLine($"Thread_ID: {Thread.CurrentThread.ManagedThreadId} , Number:{4}");
//});


//Console.WriteLine("程式執行結束");


static int binarySearch(List<long> ints, int target)
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