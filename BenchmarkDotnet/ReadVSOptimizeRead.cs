using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BenchmarkDotnet
{
    [InProcess]
    [MemoryDiagnoser]
    public class ReadVSOptimizeRead
    {
        public static string record = "19,Winonah,Ashtonhurst,washtonhursti@people.com.cn,Female,68.161.193.249";
        public static int startIndex = 0;
        public static int count = record.Length;
        public static Type PIIType = typeof(PII);
        public static PropertyInfo[] PIIprops = PIIType.GetProperties();


        delegate void SetterDelegate(object obj, object value);
        static SetterDelegate[] setters = PIIprops.Select(x => CreateSetter(x)).ToArray();

        static SetterDelegate CreateSetter(PropertyInfo propertyInfo)
        {
            var targetParm = Expression.Parameter(typeof(object));
            var targetValue = Expression.Parameter(typeof(object));

            Expression castTarget = Expression.Convert(targetParm, propertyInfo.DeclaringType);
            Expression castValue = Expression.Convert(targetValue, propertyInfo.PropertyType);

            MethodCallExpression methodCall = Expression.Call(castTarget, propertyInfo.GetSetMethod(), castValue);
            SetterDelegate setterDelegate = Expression.Lambda<SetterDelegate>(methodCall, targetParm, targetValue).Compile();

            return setterDelegate;
        }

        [Benchmark]
        public void Read()
        {
            //for (int j = 0; j < 2_500_000; j++)
            //{
            string data = "19,Winonah,Ashtonhurst,washtonhursti@people.com.cn,Female,68.161.193.249";
            string[] strings = data.Split(',');

            Type PIIType = typeof(PII);
            PropertyInfo[] PIIprops = PIIType.GetProperties();

            PII pII = new PII();

            for (int i = 0; i < PIIprops.Length; i++)
            {
                PIIprops[i].SetValue(pII, strings[i]);
            }
            //}
        }

        [Benchmark]
        public void OptimizeRead()
        {
            //for (int j = 0; j < 2_500_000; j++)
            //{
            string[] strings = new string[6];

            ReadOnlySpan<char> dataAsSpan = record.AsSpan();

            for (int i = 0; i < strings.Length; i++)
            {
                var loc = record.IndexOf(",", startIndex, count);

                string str = loc != -1 ? dataAsSpan.Slice(startIndex, loc - startIndex).ToString()
                                       : dataAsSpan.Slice(startIndex, record.Length - startIndex).ToString();

                strings[i] = str;

                if (loc == -1) break;

                startIndex = loc + 1;
                count = record.Length - loc - 1;
            }

            PII pII = new PII();

            for (int i = 0; i < PIIprops.Length; i++)
            {
                //PIIprops[i].SetValue(pII, strings[i]);
                setters[i](pII, strings[i]);
            }
            //}

            //for (int i = 0; i < PIIprops.Length; i++)
            //{
            //    var loc = data.IndexOf(",", startIndex, count);

            //    string str = loc != -1 ? dataAsSpan.Slice(startIndex, loc - startIndex).ToString()
            //                           : dataAsSpan.Slice(startIndex, data.Length - startIndex).ToString();

            //    PIIprops[i].SetValue(pII, str);

            //    if (loc == -1) break;

            //    startIndex = loc + 1;
            //    count = data.Length - loc - 1;
            //}
        }
    }
}
