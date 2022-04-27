using BenchmarkDotNet.Attributes;
using System.Numerics;

namespace PerformanceExamples
{
    [MemoryDiagnoser]
    public class ParwiseAdditionTest
    {
        private const int N = 16384;
        private readonly double[] data1;
        private readonly double[] data2;

        public ParwiseAdditionTest()
        {
            data1 = new double[N];
            data2 = new double[N];
            var random = new Random(42);
            for (int i = 0; i < N; i++)
            {
                data1[i] = random.NextDouble();
                data2[i] = random.NextDouble();
            }
        }

        [Benchmark]
        public double[] Normal()
        {
            var returnData = new double[N];
            for (int i = 0; i < N; i++)
            {
                returnData[i] = data1[i] + data2[i];
            }

            return returnData;
        }

        [Benchmark]
        public double[] Vector()
        {
            var returnData = new double[N];

            var vectorSize = Vector<double>.Count;
            for (int i = 0; i < N; i += vectorSize)
            {
                var vector1 = new Vector<double>(data1, i);
                var vector2 = new Vector<double>(data2, i);
                var resultVector = vector1 + vector2;
                resultVector.CopyTo(returnData, i);
            }

            return returnData;
        }
    }
}
