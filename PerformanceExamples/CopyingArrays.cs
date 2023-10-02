using BenchmarkDotNet.Attributes;

namespace PerformanceExamples;

[MemoryDiagnoser]
public class CopyingArrays
{
    private const int N = 16384;
    private readonly List<double> data1;

    public CopyingArrays()
    {
        data1 = new List<double>(N);
        var random = new Random(42);
        for (int i = 0; i < N; i++)
        {
            data1.Add(random.NextDouble());
        }
    }

    [Benchmark]
    public List<double> CopyWithToList()
    {
        var list = data1.ToList();
        return list;
    }

    [Benchmark]
    public List<double> CopyWithCtor()
    {
        var list = new List<double>(data1);
        return list;
    }

    [Benchmark]
    public List<double> CopyWithAddRange()
    {
        var list = new List<double>();
        list.AddRange(data1);
        return list;
    }

    [Benchmark]
    public List<double> CopyWithAddRangeSpecifySize()
    {
        var list = new List<double>(N);
        list.AddRange(data1);
        return list;
    }

    [Benchmark]
    public List<double> CopyWithForeach()
    {
        var list = new List<double>();
        foreach (var item in data1)
        {
            list.Add(item);
        }

        return list;
    }

    [Benchmark]
    public List<double> CopyWithForeachSpecifySize()
    {
        var list = new List<double>(N);
        foreach (var item in data1)
        {
            list.Add(item);
        }

        return list;
    }

    [Benchmark]
    public List<double> CopyWithFilterLinq()
    {
        var list = data1.Where(x => x > 0.5).ToList();
        return list;
    }

    [Benchmark]
    public List<double> CopyWithFilterForeach()
    {
        var list = new List<double>();
        foreach (var item in data1)
        {
            if (item > 0.5)
            {
                list.Add(item);
            }
        }
        return list;
    }

    [Benchmark]
    public List<double> CopyWithFilterForeachSpecifySize()
    {
        var list = new List<double>(N);
        foreach (var item in data1)
        {
            if (item > 0.5)
            {
                list.Add(item);
            }
        }
        return list;
    }
}
