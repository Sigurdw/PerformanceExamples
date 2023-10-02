namespace PerformanceExamplesTests;

[TestClass]
public class PerformanceExample
{
    public interface IPerson
    {
        string FirstName { get; }

        string LastName { get; }

        int Age { get; }

        decimal NetWorth { get; }

        decimal FotballInterestInPercentage { get; }
    }

    public sealed record Person(string FirstName, string LastName, int Age, decimal NetWorth, decimal FotballInterestInPercentage) : IPerson;

    public readonly record struct Person2(string FirstName, string LastName, int Age, decimal NetWorth, decimal FotballInterestInPercentage) : IPerson;

    private const int N = 16384;

    private const int Half = 5000000;

    [TestMethod]
    public void StructExamples()
    {
        Random random = new Random();
        List<IPerson> list = new List<IPerson>();
        for (int i = 0; i < N; i++)
        {
            list.Add(new Person2("First" + random.Next(1000, 9999), "Last" + random.Next(1000, 9999), random.Next(0, 150), (decimal)random.NextDouble() * 10000000, (decimal)random.NextDouble()));
        }

        var sumOfAge = SumAgeOfRichPeople(list);
        Console.WriteLine(sumOfAge);
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
}