using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BenchmarkDotnet
{
    public class Foo
    {
        /// <summary>
        /// 協力單位，用來當成串列的填充物
        /// </summary>
        public Guid Id { get; set; }
        public string Bar1 { get; set; }
        public string Bar2 { get; set; }
        public string Bar3 { get; set; }
        public string Bar4 { get; set; }
        public string Bar5 { get; set; }
    }
}
