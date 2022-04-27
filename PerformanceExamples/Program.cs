using BenchmarkDotNet.Running;
using PerformanceExamples;

//var summary = BenchmarkRunner.Run<ParwiseAdditionTest>();

//var summary = BenchmarkRunner.Run<SumOfListOfDoublesTest>();

//var summary = BenchmarkRunner.Run<SumOfListOfObjectPropertiesTest>();

//var summary = BenchmarkRunner.Run<FilterFunctionTest>();

var summary = BenchmarkRunner.Run<BoxingTest>();

Console.WriteLine(summary);
