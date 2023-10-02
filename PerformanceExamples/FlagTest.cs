using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerformanceExamples;

[Flags]
public enum FlagTestEnum
{
    Red = 1,
    Blue = 2,
    Green = 4,
    Yellow = 8,
    Purple = 16,
    Orange = 32,
}

[MemoryDiagnoser]
public class FlagTest
{
    private const int iterations = 100;

    private readonly List<FlagTestEnum> flagList;

    public FlagTest()
    {
        flagList = new List<FlagTestEnum>();
        var random = new Random(42);
        for (int i = 0; i < iterations; i++)
        {
            var number = random.Next() % 5;
            var enumNumber = (int)Math.Pow(2, number);

            flagList.Add((FlagTestEnum)enumNumber);
        }
    }

    [Benchmark]
    public int HasFlagTest()
    {
        var enumList = new List<FlagTest>(iterations);

        var count = 0;
        foreach (var flagTest in flagList)
        {
            if (flagTest.HasFlag(FlagTestEnum.Yellow))
            {
                count++;
            }
        }

        return count;
    }

    [Benchmark]
    public int OperatorTest()
    {
        var enumList = new List<FlagTest>(iterations);

        var count = 0;
        foreach (var flagTest in flagList)
        {
            if ((flagTest & FlagTestEnum.Yellow) == FlagTestEnum.Yellow)
            {
                count++;
            }
        }

        return count;
    }

}
