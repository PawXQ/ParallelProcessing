using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BenchmarkDotnet
{
    [InProcess]
    [MemoryDiagnoser]
    public class WriteVSOptimizeWrite
    {

        //public static string record = "19,Winonah,Ashtonhurst,washtonhursti@people.com.cn,Female,68.161.193.249";
        public static Type PIIType = typeof(PII);
        public static PropertyInfo[] PIIprops = PIIType.GetProperties();

        delegate object GetterDelegate(object obj);
        static StringBuilder stringBuilder = new StringBuilder(90);
        static char[] buffer = new char[90];
        static GetterDelegate[] getters = PIIprops.Select(x => CreateGetter(x)).ToArray();

        static GetterDelegate CreateGetter(PropertyInfo propertyInfo)
        {
            var targetParm = Expression.Parameter(typeof(object));

            Expression castTarget = Expression.Convert(targetParm, propertyInfo.DeclaringType);

            MethodCallExpression methodCall = Expression.Call(castTarget, propertyInfo.GetGetMethod());
            GetterDelegate getterDelegate = Expression.Lambda<GetterDelegate>(methodCall, targetParm).Compile();

            return getterDelegate;
        }

        [Benchmark]
        public void Write()
        {
            Type PIIType = typeof(PII);
            PropertyInfo[] PIIprops = PIIType.GetProperties();

            //for (int j = 0; j < 2_500_000; j++)
            //{
            PII pII = new PII()
            {
                id = "19",
                first_name = "Winonah",
                last_name = "Ashtonhurst",
                email = "washtonhursti@people.com.cn",
                gender = "Female",
                ip_address = "68.161.193.249"
            };

            string pIIstring = "";
            foreach (PropertyInfo PIIprop in PIIprops)
            {
                pIIstring += $"{PIIprop.GetValue(pII).ToString()},";
            }

            pIIstring = pIIstring.TrimEnd(',');
            //}
        }

        [Benchmark]
        public void OptimizeWrite()
        {
            //for (int j = 0; j < 2_500_000; j++)
            //{
            PII pII = new PII()
            {
                id = "19",
                first_name = "Winonah",
                last_name = "Ashtonhurst",
                email = "washtonhursti@people.com.cn",
                gender = "Female",
                ip_address = "68.161.193.249"
            };

            //List<string> pIIstrings = new List<string>();

            for (int i = 0; i < PIIprops.Length; i++)
            {
                stringBuilder.Append(getters[i](pII).ToString());
                if (i < PIIprops.Length - 1)
                {
                    stringBuilder.Append(',');
                }


                //pIIstrings.Add(PIIprops[i].GetValue(pII).ToString());
            }


            stringBuilder.CopyTo(0, buffer, 0, stringBuilder.Length);
            stringBuilder.Clear();


            //foreach (PropertyInfo PIIprop in PIIprops)
            //{

            //    pIIstrings.Add(PIIprop.GetValue(pII).ToString());
            //}

            //string pIIstring = string.Join(",", pIIstrings);
            //}
        }
    }
}
