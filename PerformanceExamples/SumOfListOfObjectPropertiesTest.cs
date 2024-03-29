﻿using BenchmarkDotNet.Attributes;

namespace PerformanceExamples;

[MemoryDiagnoser]
public class SumOfListOfObjectPropertiesTest
{
    public sealed record A(List<double> values);


    private const int N = 16;
    private const int N2 = 1024;
    private readonly List<A> data1;

    public SumOfListOfObjectPropertiesTest()
    {
        data1 = new List<A>(N);
        var random = new Random(42);
        for (int i = 0; i < N2; i++)
        {
            var innerList = new List<double>();
            for (int j = 0; j < N; j++)
            {
                innerList.Add(random.NextDouble());
            }

            data1.Add(new A(innerList));
        }
    }

    [Benchmark]
    public double SumWithLinq()
    {

        return data1.Sum(a => a.values.Sum());
    }

    [Benchmark]
    public double SumWithIEnumerableForeach()
    {
        return IEnumerableSum(data1);
    }

    private static double IEnumerableSum(IEnumerable<A> doubles)
    {
        double sum = 0;
        foreach (var a in doubles)
        {
            sum += IEnumerableSum(a.values);
        }

        return sum;
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

    private static double ListSum(List<A> doubles)
    {
        double sum = 0;
        foreach (var a in doubles)
        {
            sum += ListSum(a.values);
        }

        return sum;
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
}
