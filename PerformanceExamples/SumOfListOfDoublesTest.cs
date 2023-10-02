using BenchmarkDotNet.Attributes;

namespace PerformanceExamples;

[MemoryDiagnoser]
public class SumOfListOfDoublesTest
{
    private const int N = 16384;
    private readonly List<double> data1;

    public SumOfListOfDoublesTest()
    {
        data1 = new List<double>(N);
        var random = new Random(42);
        for (int i = 0; i < N; i++)
        {
            data1.Add(random.NextDouble());
        }
    }

    [Benchmark]
    public double SumWithLinq()
    {

        return data1.Sum();
    }

    [Benchmark]
    public double SumWithIEnumerableForeach()
    {
        return IEnumerableSum(data1);
    }

    private static double IEnumerableSum(IEnumerable<double> doubles)
    {
        double sum = 0;
        foreach (var value in doubles)
        {
            sum += value;
        }

        return sum;
    }

    [Benchmark]
    public double SumWithListForeach()
    {
        return ListSum(data1);
    }

    private static double ListSum(List<double> doubles)
    {
        double sum = 0;
        foreach (var value in doubles)
        {
            sum += value;
        }

        return sum;
    }

    [Benchmark]
    public double SumWithListFor()
    {
        return ListForSum(data1);
    }

    private static double ListForSum(List<double> doubles)
    {
        double sum = 0;
        int count = doubles.Count;
        for (int i = 0; i < count; i++)
        {
            double value = doubles[i];
            sum += value;
        }

        return sum;
    }

    [Benchmark]
    public double SumWithListPartialUnrollFor()
    {
        return ListForPartialUnrollSum(data1);
    }

    private static double ListForPartialUnrollSum(List<double> doubles)
    {
        double sum1 = 0;
        double sum2 = 0;
        double sum3 = 0;
        double sum4 = 0;
        int count = doubles.Count;
        for (int i = 0; i < count; i+=4)
        {
            double value1 = doubles[i];
            sum1 += value1;
            double value2 = doubles[i+1];
            sum2 += value2;
            double value3 = doubles[i+2];
            sum3 += value3;
            double value4 = doubles[i+3];
            sum4 += value4;
        }

        return sum1 + sum2 + sum3 + sum4;
    }

    [Benchmark]
    public double SumWithListPartialUnrollIntoSameVariableFor()
    {
        return ListForPartialUnrollIntoSameVariableSum(data1);
    }

    private static double ListForPartialUnrollIntoSameVariableSum(List<double> doubles)
    {
        double sum = 0;
        int count = doubles.Count;
        for (int i = 0; i < count; i += 4)
        {
            double value1 = doubles[i];
            sum += value1;
            double value2 = doubles[i + 1];
            sum += value2;
            double value3 = doubles[i + 2];
            sum += value3;
            double value4 = doubles[i + 3];
            sum += value4;
        }

        return sum;
    }

    [Benchmark]
    public double SumWithGenericForeach()
    {
        return GenericSum(data1);
    }

    private static double GenericSum<T>(T doubles) where T : IEnumerable<double>
    {
        double sum = 0;
        foreach (var value in doubles)
        {
            sum += value;
        }

        return sum;
    }
}
