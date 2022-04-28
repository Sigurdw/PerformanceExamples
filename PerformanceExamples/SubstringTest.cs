using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace PerformanceExamples;

[MemoryDiagnoser]
public class SubstringTest
{
    private const string Address = "http://careteam.com/sigurdwien/nextbirthday/2022-06-20/34/hurra";
    private const string FirstPart = "http://careteam.com/";

    public enum BirthdayEventType
    {
        nextbirthday,
        previousbirthday,
        actualbirthday
    }

    public sealed class BirthdayEvent
    {
        public string? User { get; set; }

        public BirthdayEventType EventType { get; set; }

        public int Day { get; set; }

        public int Month { get; set; }

        public int Year { get; set; }

        public int Age { get; set; }

        public string? CustomMessage { get; set; }
    }

    [Benchmark]
    public BirthdayEvent SimpleArgumentParserSubstring()
    {
        var parameterPart = Address.Substring(FirstPart.Length);

        var argumentStrings = parameterPart.Split('/');

        var birthDayEvent = new BirthdayEvent();

        birthDayEvent.User = argumentStrings[0];

        birthDayEvent.EventType = Enum.Parse<BirthdayEventType>(argumentStrings[1]);
        var dateString = argumentStrings[2];
        var dateParts = dateString.Split('-');
        birthDayEvent.Year = int.Parse(dateParts[0]);
        birthDayEvent.Month = int.Parse(dateParts[1]);
        birthDayEvent.Day = int.Parse(dateParts[2]);

        birthDayEvent.Age = int.Parse(argumentStrings[3]);
        birthDayEvent.CustomMessage = argumentStrings[4];

        return birthDayEvent;
    }

    [Benchmark]
    public BirthdayEvent SimpleArgumentParserSpan()
    {
        ReadOnlySpan<char> addressSpan = Address;
        var parametersSpan = addressSpan.Slice(FirstPart.Length);

        var nextIndex = parametersSpan.IndexOf('/');
        var birthDayEvent = new BirthdayEvent();

        birthDayEvent.User = parametersSpan.Slice(0, nextIndex).ToString();

        parametersSpan = parametersSpan.Slice(nextIndex + 1);
        nextIndex = parametersSpan.IndexOf('/');

        var eventTypeSpan = parametersSpan.Slice(0, nextIndex);
        birthDayEvent.EventType = Enum.Parse<BirthdayEventType>(eventTypeSpan);

        parametersSpan = parametersSpan.Slice(nextIndex + 1);
        nextIndex = parametersSpan.IndexOf('/');

        var dateSpan = parametersSpan.Slice(0, nextIndex);
        var dateTime = DateTime.Parse(dateSpan);
        birthDayEvent.Year = dateTime.Year;
        birthDayEvent.Month = dateTime.Month;
        birthDayEvent.Day = dateTime.Day;

        parametersSpan = parametersSpan.Slice(nextIndex + 1);
        nextIndex = parametersSpan.IndexOf('/');

        var ageSpan = parametersSpan.Slice(0, nextIndex);
        birthDayEvent.Age = int.Parse(ageSpan);

        parametersSpan = parametersSpan.Slice(nextIndex + 1);

        birthDayEvent.CustomMessage = parametersSpan.ToString();

        return birthDayEvent;
    }
}
