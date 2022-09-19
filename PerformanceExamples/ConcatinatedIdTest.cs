using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerformanceExamples;

public sealed record Item(Guid LowLevelId, Guid HighLevelId, string Payload, string Payload2);

[MemoryDiagnoser]
public class ConcatinatedIdTest
{
    private const int iterations = 10000;

    [Benchmark]
    public void ConcatinatedStringKey()
    {
        var dictionary = new Dictionary<string, Item>();
        var keyList = new List<string>(iterations);
        for (int i = 0; i < iterations; i++)
        {
            var item = new Item(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid().ToString(), Guid.NewGuid().ToString());

            var key = $"{item.LowLevelId}_{item.HighLevelId}";
            dictionary.Add(key, item);
            keyList.Add(key);
        }

        var valueList = new List<Item>(iterations);
        foreach (var key in keyList)
        {
            valueList.Add(dictionary[key]);
        }
    }

    [Benchmark]
    public void ConcatinatedTupleKey()
    {
        var dictionary = new Dictionary<(Guid, Guid), Item>();
        var keyList = new List<(Guid, Guid)>(iterations);
        for (int i = 0; i < iterations; i++)
        {
            var item = new Item(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid().ToString(), Guid.NewGuid().ToString());

            var key = (item.LowLevelId, item.HighLevelId);
            dictionary.Add(key, item);
            keyList.Add(key);
        }

        var valueList = new List<Item>(iterations);
        foreach (var key in keyList)
        {
            valueList.Add(dictionary[key]);
        }
    }
}
