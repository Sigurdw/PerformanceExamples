using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerformanceExamples;

[MemoryDiagnoser]
public class BoxingTest
{
    public interface IPerson
    {
        string FirstName { get; }

        string LastName { get;}

        int Age { get; }

        decimal NetWorth { get; }

        decimal FotballInterestInPercentage { get; }
    }

    public sealed record Person(string FirstName, string LastName, int Age, decimal NetWorth, decimal FotballInterestInPercentage) : IPerson;

    public readonly record struct Person2(string FirstName, string LastName, int Age, decimal NetWorth, decimal FotballInterestInPercentage) : IPerson;

    private const int N = 16384;

    private const int Half = 5000000;

    [Benchmark]
    public int ClassExample()
    {
        Random random = new Random();
        List<Person> list = new List<Person>();
        for (int i = 0; i < N; i++)
        {
            list.Add(new Person("First" + random.Next(1000, 9999), "Last" + random.Next(1000, 9999), random.Next(0, 150), (decimal)random.NextDouble() * 10000000, (decimal)random.NextDouble()));
        }

        var sumOfAge = SumAgeOfRichPeople(list);
        return sumOfAge;
    }

    public static int SumAgeOfRichPeople(IEnumerable<IPerson> people)
    {
        int sumOfAge = 0;
        foreach (var person in people)
        {
            sumOfAge += GetAgeFromRichPerson(person);
        }

        return sumOfAge;
    }

    public static int GetAgeFromRichPerson(IPerson person)
    {
        if (IsPersonRich(person))
        {
            return person.Age;
        }

        return 0;
    }

    public static bool IsPersonRich(IPerson person)
    {
        return person.NetWorth > Half;
    }

    [Benchmark]
    public int StructExample()
    {
        Random random = new Random();
        List<IPerson> list = new List<IPerson>();
        for (int i = 0; i < N; i++)
        {
            list.Add(new Person2("First" + random.Next(1000, 9999), "Last" + random.Next(1000, 9999), random.Next(0, 150), (decimal)random.NextDouble() * 10000000, (decimal)random.NextDouble()));
        }

        var sumOfAge = SumAgeOfRichPeople(list);
        return sumOfAge;
    }

    [Benchmark]
    public int StructExample2()
    {
        Random random = new Random();
        List<Person2> list = new List<Person2>();
        for (int i = 0; i < N; i++)
        {
            list.Add(new Person2("First" + random.Next(1000, 9999), "Last" + random.Next(1000, 9999), random.Next(0, 150), (decimal)random.NextDouble() * 10000000, (decimal)random.NextDouble()));
        }

        var sumOfAge = SumAgeOfRichPeople2(list);
        return sumOfAge;
    }

    public static int SumAgeOfRichPeople2<T>(IEnumerable<T> people) where T : IPerson
    {
        int sumOfAge = 0;
        foreach (var person in people)
        {
            sumOfAge += GetAgeFromRichPerson2(person);
        }

        return sumOfAge;
    }

    public static int GetAgeFromRichPerson2<T>(T person) where T : IPerson
    {
        if (IsPersonRich2(person))
        {
            return person.Age;
        }

        return 0;
    }

    public static bool IsPersonRich2<T>(T person) where T : IPerson
    {
        return person.NetWorth > Half;
    }

    [Benchmark]
    public int StructExample3()
    {
        Random random = new Random();
        List<Person2> list = new List<Person2>();
        for (int i = 0; i < N; i++)
        {
            list.Add(new Person2("First" + random.Next(1000, 9999), "Last" + random.Next(1000, 9999), random.Next(0, 150), (decimal)random.NextDouble() * 10000000, (decimal)random.NextDouble()));
        }

        var sumOfAge = SumAgeOfRichPeople3(list);
        return sumOfAge;
    }

    public static int SumAgeOfRichPeople3(IEnumerable<Person2> people)
    {
        int sumOfAge = 0;
        foreach (var person in people)
        {
            sumOfAge += GetAgeFromRichPerson3(person);
        }

        return sumOfAge;
    }

    public static int GetAgeFromRichPerson3(Person2 person)
    {
        if (IsPersonRich3(person))
        {
            return person.Age;
        }

        return 0;
    }

    public static bool IsPersonRich3(Person2 person)
    {
        return person.NetWorth > Half;
    }

    [Benchmark]
    public int StructExample4()
    {
        Random random = new Random();
        List<Person2> list = new List<Person2>();
        for (int i = 0; i < N; i++)
        {
            list.Add(new Person2("First" + random.Next(1000, 9999), "Last" + random.Next(1000, 9999), random.Next(0, 150), (decimal)random.NextDouble() * 10000000, (decimal)random.NextDouble()));
        }

        var sumOfAge = SumAgeOfRichPeople4(list);
        return sumOfAge;
    }

    public static int SumAgeOfRichPeople4(IEnumerable<Person2> people)
    {
        int sumOfAge = 0;
        foreach (var person in people)
        {
            sumOfAge += GetAgeFromRichPerson4(person);
        }

        return sumOfAge;
    }

    public static int GetAgeFromRichPerson4(in Person2 person)
    {
        if (IsPersonRich4(person))
        {
            return person.Age;
        }

        return 0;
    }

    public static bool IsPersonRich4(in Person2 person)
    {
        return person.NetWorth > Half;
    }

    [Benchmark]
    public int StringExample()
    {
        Random random = new Random();
        List<Person2> list = new List<Person2>();
        Span<char> firstNameArray = stackalloc char[9];
        Span<char> lastNameArray = stackalloc char[8];
        for (int i = 0; i < N; i++)
        {
            firstNameArray[0] = 'F';
            firstNameArray[1] = 'i';
            firstNameArray[2] = 'r';
            firstNameArray[3] = 's';
            firstNameArray[4] = 't';
            var firstRandom = random.Next(1000, 9999);
            firstRandom.TryFormat(firstNameArray.Slice(5), out _);

            lastNameArray[0] = 'L';
            lastNameArray[1] = 'a';
            lastNameArray[2] = 's';
            lastNameArray[3] = 't';
            var lastRandom = random.Next(1000, 9999);
            lastRandom.TryFormat(lastNameArray.Slice(4), out _);

            list.Add(new Person2(new string(firstNameArray), new string(lastNameArray), random.Next(0, 150), (decimal)random.NextDouble() * 10000000, (decimal)random.NextDouble()));
        }

        var sumOfAge = SumAgeOfRichPeople4(list);
        return sumOfAge;
    }
}
