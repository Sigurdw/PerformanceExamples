using BenchmarkDotNet.Running;
using PerformanceExamples;

//var summary = BenchmarkRunner.Run<ParwiseAdditionTest>();

//var summary = BenchmarkRunner.Run<SumOfListOfDoublesTest>();

//var summary = BenchmarkRunner.Run<SumOfListOfObjectPropertiesTest>();

//var summary = BenchmarkRunner.Run<FilterFunctionTest>();

//var summary = BenchmarkRunner.Run<BoxingTest>();

//var summary = BenchmarkRunner.Run<ConcurrentDictionaryTest>();

//var summary = BenchmarkRunner.Run<ConcurrentDictionaryThreadedTest>();

//var summary = BenchmarkRunner.Run<ConcatinatedIdTest>();

var summary = BenchmarkRunner.Run<ConcurrentDictionaryThreadedWithReadTest>();

//var summary = BenchmarkRunner.Run<SubstringTest>();

Console.WriteLine(summary);
