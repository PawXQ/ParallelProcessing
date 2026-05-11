using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BenchmarkDotnet
{
    [InProcess]
    [MemoryDiagnoser]
    public class Count
    {
        public static List<int> countList = new List<int>() { 1, 2, 3, 4, 5 };

        [Benchmark]
        public void CountMethod()
        {
            int num = countList.Count();
        }

        [Benchmark]
        public void CountProp()
        {
            int num = countList.Count;
        }
    }
}
