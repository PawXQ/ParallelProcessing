### 優化項目 1-1(Write): 多一個 if, 少 TrimEnd

| Method        |     Mean |    Error |   StdDev |   Median |   Gen0 | Allocated |
| ------------- | -------: | -------: | -------: | -------: | -----: | --------: |
| Write         | 986.5 ns | 43.79 ns | 129.1 ns | 969.4 ns | 0.1450 |     769 B |
| OptimizeWrite | 967.2 ns | 43.90 ns | 127.4 ns | 923.8 ns | 0.1163 |     617 B |

```C#
[Benchmark]
public void Write()
{
    Type PIIType = typeof(PII);
    PropertyInfo[] PIIprops = PIIType.GetProperties();

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
}

[Benchmark]
public void OptimizeWrite()
{
    Type PIIType = typeof(PII);
    PropertyInfo[] PIIprops = PIIType.GetProperties();

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
        if (pIIstring == "")
        {
            pIIstring = PIIprop.GetValue(pII).ToString();
        }
        pIIstring += $",{PIIprop.GetValue(pII).ToString()}";
    }
}
```

### 優化項目 1-2(Read): 使用 String builder

| Method        |     Mean |    Error |   StdDev |   Gen0 | Allocated |
| ------------- | -------: | -------: | -------: | -----: | --------: |
| Write         | 819.3 ns | 16.13 ns | 19.80 ns | 0.1459 |     769 B |
| OptimizeWrite | 792.2 ns | 12.58 ns | 11.77 ns | 0.0887 |     469 B |

```C#
public static StringBuilder pIIStringBuilder = new StringBuilder();

[Benchmark]
public void Write()
{
    Type PIIType = typeof(PII);
    PropertyInfo[] PIIprops = PIIType.GetProperties();

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
}

[Benchmark]
public void OptimizeWrite()
{
    Type PIIType = typeof(PII);
    PropertyInfo[] PIIprops = PIIType.GetProperties();

    PII pII = new PII()
    {
        id = "19",
        first_name = "Winonah",
        last_name = "Ashtonhurst",
        email = "washtonhursti@people.com.cn",
        gender = "Female",
        ip_address = "68.161.193.249"
    };


    foreach (PropertyInfo PIIprop in PIIprops)
    {
        if (pIIStringBuilder.Length == 0)
        {
            pIIStringBuilder.Append(PIIprop.GetValue(pII).ToString());
        }
        pIIStringBuilder.Append($",{PIIprop.GetValue(pII).ToString()}");
    }

    string pIIstring = pIIStringBuilder.ToString();

    pIIStringBuilder.Clear();
}
```

### 優化項目 1-3(Write): 使用 string.Join(",", List)

| Method        |     Mean |    Error |   StdDev |   Gen0 | Allocated |
| ------------- | -------: | -------: | -------: | -----: | --------: |
| Write         | 811.9 ns | 14.83 ns | 13.15 ns | 0.1459 |     769 B |
| OptimizeWrite | 743.3 ns | 10.65 ns |  9.96 ns | 0.0658 |     349 B |

```C#
[Benchmark]
public void Write()
{
    Type PIIType = typeof(PII);
    PropertyInfo[] PIIprops = PIIType.GetProperties();

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
}

[Benchmark]
public void OptimizeWrite()
{
    Type PIIType = typeof(PII);
    PropertyInfo[] PIIprops = PIIType.GetProperties();

    PII pII = new PII()
    {
        id = "19",
        first_name = "Winonah",
        last_name = "Ashtonhurst",
        email = "washtonhursti@people.com.cn",
        gender = "Female",
        ip_address = "68.161.193.249"
    };

    List<string> pIIstrings = new List<string>();
    foreach (PropertyInfo PIIprop in PIIprops)
    {
        pIIstrings.Add(PIIprop.GetValue(pII).ToString());
    }

    string pIIstring = string.Join(",", pIIstrings);
}
```

### 優化項目 1-4(Write): 使用 string.Join(",", List) & Reflection

| Method        |     Mean |   Error |  StdDev |   Gen0 | Allocated |
| ------------- | -------: | ------: | ------: | -----: | --------: |
| Write         | 761.6 ns | 1.38 ns | 1.29 ns | 0.1459 |     769 B |
| OptimizeWrite | 627.9 ns | 1.35 ns | 1.27 ns | 0.0591 |     312 B |

```C#
[Benchmark]
public void Write()
{
    Type PIIType = typeof(PII);
    PropertyInfo[] PIIprops = PIIType.GetProperties();

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
}

[Benchmark]
public void OptimizeWrite()
{
    PII pII = new PII()
    {
        id = "19",
        first_name = "Winonah",
        last_name = "Ashtonhurst",
        email = "washtonhursti@people.com.cn",
        gender = "Female",
        ip_address = "68.161.193.249"
    };

    List<string> pIIstrings = new List<string>();

    for (int i = 0; i < PIIprops.Length; i++)
    {
        pIIstrings.Add(PIIprops[i].GetValue(pII).ToString());
    }

    string pIIstring = string.Join(",", pIIstrings);
}
```

### 優化項目 1-5(Write): 使用 string.Join(",", List) & Rflection & Delegate & `Direct Get Value`

| Method        |     Mean |   Error |  StdDev |   Gen0 | Allocated |
| ------------- | -------: | ------: | ------: | -----: | --------: |
| Write         | 761.9 ns | 2.63 ns | 2.33 ns | 0.1459 |     769 B |
| OptimizeWrite | 254.9 ns | 1.53 ns | 1.43 ns | 0.0591 |     312 B |

```C#
public static Type PIIType = typeof(PII);
public static PropertyInfo[] PIIprops = PIIType.GetProperties();

delegate object GetterDelegate(object obj);

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
}

[Benchmark]
public void OptimizeWrite()
{
    PII pII = new PII()
    {
        id = "19",
        first_name = "Winonah",
        last_name = "Ashtonhurst",
        email = "washtonhursti@people.com.cn",
        gender = "Female",
        ip_address = "68.161.193.249"
    };

    List<string> pIIstrings = new List<string>();

    for (int i = 0; i < PIIprops.Length; i++)
    {
        pIIstrings.Add(getters[i](pII).ToString());
    }

    string pIIstring = string.Join(",", pIIstrings);
}
```

### 優化項目 1-6(Write): 使用 `StringBuilder & budder` & Rflection & Delegate & `Direct Get Value`

#### 測量單位 for loop 250W

| Method        |       Mean |    Error |   StdDev |        Gen0 |  Allocated |
| ------------- | ---------: | -------: | -------: | ----------: | ---------: |
| Write         | 1,797.1 ms | 32.90 ms | 30.78 ms | 349000.0000 | 1747.81 MB |
| OptimizeWrite |   307.6 ms |  2.56 ms |  2.27 ms |  15000.0000 |    76.4 MB |

#### 測量單位 1 time

| Method        |     Mean |   Error |  StdDev |   Gen0 | Allocated |
| ------------- | -------: | ------: | ------: | -----: | --------: |
| Write         | 778.0 ns | 4.75 ns | 3.96 ns | 0.1459 |     769 B |
| OptimizeWrite | 121.6 ns | 0.22 ns | 0.18 ns | 0.0060 |      32 B |

```C#
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

    for (int i = 0; i < PIIprops.Length; i++)
    {
        stringBuilder.Append(getters[i](pII).ToString());
        if (i < PIIprops.Length - 1)
        {
            stringBuilder.Append(',');
        }
    }


    stringBuilder.CopyTo(0, buffer, 0, stringBuilder.Length);
    stringBuilder.Clear();
    //}
}
```
