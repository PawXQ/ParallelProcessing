### 優化項目 1-1(Read): 單純使用 Split & Span

| Method       |     Mean |   Error |  StdDev |   Gen0 | Allocated |
| ------------ | -------: | ------: | ------: | -----: | --------: |
| Read         | 180.6 ns | 3.48 ns | 3.09 ns | 0.1099 |     577 B |
| OptimizeRead | 594.8 ns | 1.85 ns | 1.73 ns | 0.0496 |     260 B |

```C#
[Benchmark]
public void Read()
{
    string data = "19,Winonah,Ashtonhurst,washtonhursti@people.com.cn,Female,68.161.193.249";
    string[] strings = data.Split(',');
}

[Benchmark]
public void OptimizeRead()
{
    string data = "19,Winonah,Ashtonhurst,washtonhursti@people.com.cn,Female,68.161.193.249";
    int startIndex = 0;
    int count = data.Length;

    string[] strings = new string[6];

    ReadOnlySpan<char> dataAsSpan = data.AsSpan();

    for (int i = 0; i < strings.Length; i++)
    {
        var loc = data.IndexOf(",", startIndex, count);

        string str = loc != -1 ? dataAsSpan.Slice(startIndex, loc - startIndex).ToString()
                               : dataAsSpan.Slice(startIndex, data.Length - startIndex).ToString();

        strings[i] = str;

        if (loc == -1) break;

        startIndex = loc + 1;
        count = data.Length - loc - 1;
    }
}
```

### 優化項目 1-2(Read): 單純使用 Split & Span & `Reflection`

| Method       |       Mean |     Error |    StdDev |     Median |   Gen0 | Allocated |
| ------------ | ---------: | --------: | --------: | ---------: | -----: | --------: |
| Read         |   979.8 ns |   7.35 ns |   6.51 ns |   979.1 ns | 0.1583 |     837 B |
| OptimizeRead | 1,677.9 ns | 118.16 ns | 348.40 ns | 1,474.5 ns | 0.0992 |     521 B |

```C#
[Benchmark]
public void Read()
{
    string data = "19,Winonah,Ashtonhurst,washtonhursti@people.com.cn,Female,68.161.193.249";
    string[] strings = data.Split(',');

    Type PIIType = typeof(PII);
    PropertyInfo[] PIIprops = PIIType.GetProperties();

    PII pII = new PII();

    for (int i = 0; i < PIIprops.Length; i++)
    {
        PIIprops[i].SetValue(pII, strings[i]);
    }
}

[Benchmark]
public void OptimizeRead()
{
    string data = "19,Winonah,Ashtonhurst,washtonhursti@people.com.cn,Female,68.161.193.249";
    int startIndex = 0;
    int count = data.Length;

    string[] strings = new string[6];

    ReadOnlySpan<char> dataAsSpan = data.AsSpan();

    for (int i = 0; i < strings.Length; i++)
    {
        var loc = data.IndexOf(",", startIndex, count);

        string str = loc != -1 ? dataAsSpan.Slice(startIndex, loc - startIndex).ToString()
                                : dataAsSpan.Slice(startIndex, data.Length - startIndex).ToString();

        strings[i] = str;

        if (loc == -1) break;

        startIndex = loc + 1;
        count = data.Length - loc - 1;
    }

    Type PIIType = typeof(PII);
    PropertyInfo[] PIIprops = PIIType.GetProperties();

    PII pII = new PII();

    for (int i = 0; i < PIIprops.Length; i++)
    {
        PIIprops[i].SetValue(pII, strings[i]);
    }
}
```

### 優化項目 1-3(Read): 單純使用 Split & Span & Reflection & `Static`

| Method       |     Mean |   Error |  StdDev |   Gen0 | Allocated |
| ------------ | -------: | ------: | ------: | -----: | --------: |
| Read         | 983.7 ns | 5.72 ns | 4.78 ns | 0.1583 |     837 B |
| OptimizeRead | 776.4 ns | 3.85 ns | 3.60 ns | 0.0572 |     304 B |

```C#
public static string record = "19,Winonah,Ashtonhurst,washtonhursti@people.com.cn,Female,68.161.193.249";
public static int startIndex = 0;
public static int count = record.Length;
public static Type PIIType = typeof(PII);
public static PropertyInfo[] PIIprops = PIIType.GetProperties();

[Benchmark]
public void Read()
{
    string data = "19,Winonah,Ashtonhurst,washtonhursti@people.com.cn,Female,68.161.193.249";
    string[] strings = data.Split(',');

    Type PIIType = typeof(PII);
    PropertyInfo[] PIIprops = PIIType.GetProperties();

    PII pII = new PII();

    for (int i = 0; i < PIIprops.Length; i++)
    {
        PIIprops[i].SetValue(pII, strings[i]);
    }
}

[Benchmark]
public void OptimizeRead()
{
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
        PIIprops[i].SetValue(pII, strings[i]);
    }
}
```

### 優化項目 1-4(Read): 單純使用 Split & Span & Reflection & Static & `Delegate`

| Method       |     Mean |    Error |   StdDev |   Median |   Gen0 | Allocated |
| ------------ | -------: | -------: | -------: | -------: | -----: | --------: |
| Read         | 954.5 ns | 12.62 ns | 11.18 ns | 955.0 ns | 0.1583 |     837 B |
| OptimizeRead | 228.1 ns | 16.59 ns | 48.91 ns | 251.8 ns | 0.0212 |     112 B |

```C#
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
    string data = "19,Winonah,Ashtonhurst,washtonhursti@people.com.cn,Female,68.161.193.249";
    string[] strings = data.Split(',');

    Type PIIType = typeof(PII);
    PropertyInfo[] PIIprops = PIIType.GetProperties();

    PII pII = new PII();

    for (int i = 0; i < PIIprops.Length; i++)
    {
        PIIprops[i].SetValue(pII, strings[i]);
    }
}

[Benchmark]
public void OptimizeRead()
{
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
        setters[i](pII, strings[i]);
    }
}
```

### 優化項目 1-5(Read): 單純使用 Split & Span & Reflection & Static & Delegate & `Direct Set Value`

| Method       |     Mean |    Error |   StdDev |   Gen0 | Allocated |
| ------------ | -------: | -------: | -------: | -----: | --------: |
| Read         | 961.0 ns | 16.25 ns | 14.41 ns | 0.1583 |     837 B |
| OptimizeRead | 115.3 ns |  1.23 ns |  1.09 ns | 0.0143 |      76 B |

```C#
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
    string data = "19,Winonah,Ashtonhurst,washtonhursti@people.com.cn,Female,68.161.193.249";
    string[] strings = data.Split(',');

    Type PIIType = typeof(PII);
    PropertyInfo[] PIIprops = PIIType.GetProperties();

    PII pII = new PII();

    for (int i = 0; i < PIIprops.Length; i++)
    {
        PIIprops[i].SetValue(pII, strings[i]);
    }
}

[Benchmark]
public void OptimizeRead()
{
    ReadOnlySpan<char> dataAsSpan = record.AsSpan();
    PII pII = new PII();

    for (int i = 0; i < PIIprops.Length; i++)
    {
        var loc = record.IndexOf(",", startIndex, count);

        string str = loc != -1 ? dataAsSpan.Slice(startIndex, loc - startIndex).ToString()
                               : dataAsSpan.Slice(startIndex, record.Length - startIndex).ToString();

        setters[i](pII, str);

        if (loc == -1) break;

        startIndex = loc + 1;
        count = record.Length - loc - 1;
    }
}
```
