using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerformanceExamples;

public sealed class NormalMap
{
    private readonly object lockObject = new ();

    private readonly Dictionary<string, string> map = new ();

    public void Add(string key, string value)
    {
        lock (lockObject)
        {
            map.Add(key, value);
        }
    }

    public string GetValue(string key)
    {
        lock (lockObject)
        {
            return map[key];
        }
    }

    public List<string> GetAllKeys()
    {
        lock (lockObject)
        {
            return map.Keys.ToList();
        }
    }
}

public sealed class ConcurrentMap
{
    private readonly ConcurrentDictionary<string, string> map = new ();

    public void Add(string key, string value)
    {
        map.AddOrUpdate(key, value, (newValue, oldValue) => newValue);
    }

    public string GetValue(string key)
    {
        return map[key];
    }

    public List<string> GetAllKeys()
    {
        return map.Keys.ToList();
    }
}

[MemoryDiagnoser]
public class ConcurrentDictionaryTest
{
    private const int iterations = 10000;

    [Benchmark]
    public void NormalDictionaryWithLockAddItem()
    {
        var map = new NormalMap();
        var keyList = new List<string>(iterations);
        for (int i = 0; i < iterations; i++)
        {
            var key = Guid.NewGuid().ToString();
            var value = Guid.NewGuid().ToString();
            keyList.Add(key);
            map.Add(key, value);
        }

        foreach (var key in keyList)
        {
            map.GetValue(key);
        }
    }

    [Benchmark]
    public void ConcurrentDictionaryAddItem()
    {
        var map = new ConcurrentMap();
        var keyList = new List<string>(iterations);
        for (int i = 0; i < iterations; i++)
        {
            var key = Guid.NewGuid().ToString();
            var value = Guid.NewGuid().ToString();
            keyList.Add(key);
            map.Add(key, value);
        }

        foreach (var key in keyList)
        {
            map.GetValue(key);
        }
    }
}

[MemoryDiagnoser]
public class ConcurrentDictionaryThreadedTest
{
    private const int iterations = 10000;
    private const int numberOfThreads = 16;

    [Benchmark]
    public void NormalDictionaryWithLockAddItem()
    {
        var map = new NormalMap();

        var tasks = new Task[numberOfThreads];
        for (int i = 0; i < numberOfThreads; i++)
        {
            tasks[i] = Task.Run(() => RunMapAdd(map, iterations / numberOfThreads));
        }

        Task.WaitAll(tasks);
    }

    private static void RunMapAdd(NormalMap map, int number)
    {
        for (int i = 0; i < number; i++)
        {
            var key = Guid.NewGuid().ToString();
            var value = Guid.NewGuid().ToString();
            map.Add(key, value);
        }
    }

    [Benchmark]
    public void ConcurrentDictionaryAddItem()
    {
        var map = new ConcurrentMap();

        var tasks = new Task[numberOfThreads];
        for (int i = 0; i < numberOfThreads; i++)
        {
            tasks[i] = Task.Run(() => RunMapAdd(map, iterations / numberOfThreads));
        }

        Task.WaitAll(tasks);
    }

    private static void RunMapAdd(ConcurrentMap map, int number)
    {
        for (int i = 0; i < number; i++)
        {
            var key = Guid.NewGuid().ToString();
            var value = Guid.NewGuid().ToString();
            map.Add(key, value);
        }
    }
}

[MemoryDiagnoser]
public class ConcurrentDictionaryThreadedWithReadTest
{
    private const int iterations = 10000;
    private const int numberOfThreads = 8;

    [Benchmark]
    public void NormalDictionaryWithLockAddItem()
    {
        var map = new NormalMap();

        var tasks = new Task<List<string>>[numberOfThreads];
        for (int i = 0; i < numberOfThreads; i++)
        {
            tasks[i] = Task.Run(() => RunMapAdd(map, iterations / numberOfThreads));
        }

        var keyList = new List<string>(iterations);
        foreach (var task in tasks)
        {
            keyList.AddRange(task.Result);
        }

        var readTasks = new Task[numberOfThreads];
        for (int i = 0; i < numberOfThreads; i++)
        {
            readTasks[i] = Task.Run(() =>
            {
                var values = new List<string>(iterations);
                foreach (var key in keyList)
                {
                    var value = map.GetValue(key);
                    values.Add(value);
                }
            });
        }
    }

    private static List<string> RunMapAdd(NormalMap map, int number)
    {
        var keyList = new List<string>(number);
        for (int i = 0; i < number; i++)
        {
            var key = Guid.NewGuid().ToString();
            var value = Guid.NewGuid().ToString();
            map.Add(key, value);
            keyList.Add(key);
        }

        return keyList;
    }

    [Benchmark]
    public void ConcurrentDictionaryAddItem()
    {
        var map = new ConcurrentMap();

        var tasks = new Task<List<string>>[numberOfThreads];
        for (int i = 0; i < numberOfThreads; i++)
        {
            tasks[i] = Task.Run(() => RunMapAdd(map, iterations / numberOfThreads));
        }

        var keyList = new List<string>(iterations);
        foreach (var task in tasks)
        {
            keyList.AddRange(task.Result);
        }

        var readTasks = new Task[numberOfThreads];
        for (int i = 0; i < numberOfThreads; i++)
        {
            readTasks[i] = Task.Run(() =>
            {
                var values = new List<string>(iterations);
                foreach (var key in keyList)
                {
                    var value = map.GetValue(key);
                    values.Add(value);
                }
            });
        }
    }

    private static List<string> RunMapAdd(ConcurrentMap map, int number)
    {
        var keyList = new List<string>(number);
        for (int i = 0; i < number; i++)
        {
            var key = Guid.NewGuid().ToString();
            var value = Guid.NewGuid().ToString();
            map.Add(key, value);
            keyList.Add(key);
        }

        return keyList;
    }
}
