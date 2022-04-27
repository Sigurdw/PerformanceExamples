using BenchmarkDotNet.Attributes;

namespace PerformanceExamples;

[MemoryDiagnoser]
public class FilterFunctionTest
{
    public enum WorkType
    {
        Unknown,
        Programmer,
        Tester,
        Manager,
        HR,
    }

    public record Worker(string name, decimal salary, WorkType workType);

    public record WorkerSummary(Worker worker, decimal salaryPercentageOfWorkType);

    private const int N = 16384;
    private readonly List<Worker> workers;

    private const int halfSalary = 5000000;

    public FilterFunctionTest()
    {
        var random = new Random(42);
        workers = new List<Worker>(N);
        for (int i = 0; i < N; i++)
        {
            var name = $"Worker{random.Next()}";
            var salary = random.NextDouble() * 10000000;
            var workType = (WorkType)(random.Next() % 5);
            workers.Add(new Worker(name, (decimal)salary, workType));
        }
    }

    [Benchmark]
    public List<WorkerSummary> IEnumerableFilterLinqUseToListTest()
    {
        var hrWorkers = GetHrWorkersLinqUseToList(workers);
        var richHrWorkers = GetRichWorkersLinqUseToList(hrWorkers);

        decimal sumOfRichHrSalaries = richHrWorkers.Sum(x => x.salary);

        var workerSummaries = new List<WorkerSummary>(richHrWorkers.Count());
        foreach (var richHrWorker in richHrWorkers)
        {
            workerSummaries.Add(new WorkerSummary(richHrWorker, richHrWorker.salary / sumOfRichHrSalaries));
        }

        return workerSummaries;
    }

    private static IEnumerable<Worker> GetHrWorkersLinqUseToList(IEnumerable<Worker> workers)
    {
        return workers.Where(x => x.workType == WorkType.HR).ToList();
    }

    private static IEnumerable<Worker> GetRichWorkersLinqUseToList(IEnumerable<Worker> workers)
    {
        return workers.Where(x => x.salary > halfSalary).ToList();
    }

    [Benchmark]
    public List<WorkerSummary> IEnumerableFilterLinqtTest()
    {
        var hrWorkers = GetHrWorkersLinq(workers);
        var richHrWorkers = GetRichWorkersLinq(hrWorkers);

        decimal sumOfRichHrSalaries = richHrWorkers.Sum(x => x.salary);

        var workerSummaries = new List<WorkerSummary>(richHrWorkers.Count());
        foreach (var richHrWorker in richHrWorkers)
        {
            workerSummaries.Add(new WorkerSummary(richHrWorker, richHrWorker.salary / sumOfRichHrSalaries));
        }

        return workerSummaries;
    }

    private static IEnumerable<Worker> GetHrWorkersLinq(IEnumerable<Worker> workers)
    {
        return workers.Where(x => x.workType == WorkType.HR);
    }

    private static IEnumerable<Worker> GetRichWorkersLinq(IEnumerable<Worker> workers)
    {
        return workers.Where(x => x.salary > halfSalary);
    }

    [Benchmark]
    public List<WorkerSummary> IEnumerableFilterLinqTwoPassForeachTest()
    {
        var hrWorkers = GetHrWorkersLinq(workers);
        var richHrWorkers = GetRichWorkersLinq(hrWorkers);

        decimal sumOfRichHrSalaries = 0;
        int count = 0;
        foreach (var item in richHrWorkers)
        {
            sumOfRichHrSalaries += item.salary;
            count++;
        }

        var workerSummaries = new List<WorkerSummary>(count);
        foreach (var richHrWorker in richHrWorkers)
        {
            workerSummaries.Add(new WorkerSummary(richHrWorker, richHrWorker.salary / sumOfRichHrSalaries));
        }

        return workerSummaries;
    }

    [Benchmark]
    public List<WorkerSummary> IntegratedFilterTwoListOnePassTest()
    {
        List<Worker> richHRWorkers = new List<Worker>();
        decimal sumOfRichHrSalaries = 0;
        foreach (var worker in workers)
        {
            if (worker.workType == WorkType.HR && worker.salary > halfSalary)
            {
                sumOfRichHrSalaries += worker.salary;
                richHRWorkers.Add(worker);
            }
        }

        var workerSummaries = new List<WorkerSummary>(richHRWorkers.Count);
        foreach (var richHrWorker in richHRWorkers)
        {
            workerSummaries.Add(new WorkerSummary(richHrWorker, richHrWorker.salary / sumOfRichHrSalaries));
        }

        return workerSummaries;
    }

    [Benchmark]
    public List<WorkerSummary> IntegratedFilterOneListTwoPassesTest()
    {
        decimal sumOfRichHrSalaries = 0;
        int count = 0;
        foreach (var worker in workers)
        {
            if (worker.workType == WorkType.HR && worker.salary > halfSalary)
            {
                sumOfRichHrSalaries += worker.salary;
                count++;
            }
        }

        var workerSummaries = new List<WorkerSummary>(count);
        foreach (var worker in workers)
        {
            if (worker.workType == WorkType.HR && worker.salary > halfSalary)
            {
                workerSummaries.Add(new WorkerSummary(worker, worker.salary / sumOfRichHrSalaries));
            }
        }

        return workerSummaries;
    }

    [Benchmark]
    public List<WorkerSummary> IntegratedFilterOneListOnePassTest()
    {
        decimal sumOfRichHrSalaries = 0;
        int count = 0;
        Span<int> indecies = stackalloc int[workers.Count];
        for (int i = 0; i < workers.Count; i++)
        {
            var worker = workers[i];
            if (worker.workType == WorkType.HR && worker.salary > halfSalary)
            {
                sumOfRichHrSalaries += worker.salary;
                indecies[0] = i;
                count++;
            }
        }

        var workerSummaries = new List<WorkerSummary>(count);
        for (int i = 0; i < count; i++)
        {
            var index = indecies[i];
            var worker = workers[index];
            workerSummaries.Add(new WorkerSummary(worker, worker.salary / sumOfRichHrSalaries));
        }

        return workerSummaries;
    }
}
