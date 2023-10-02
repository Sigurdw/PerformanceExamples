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
        var managerWorkers = GetManagerWorkersLinqUseToList(workers);
        var richManagerWorkers = GetRichWorkersLinqUseToList(managerWorkers);

        decimal sumOfRichManagerSalaries = richManagerWorkers.Sum(x => x.salary);

        var workerSummaries = new List<WorkerSummary>(richManagerWorkers.Count());
        foreach (var richManagerWorker in richManagerWorkers)
        {
            workerSummaries.Add(new WorkerSummary(richManagerWorker, richManagerWorker.salary / sumOfRichManagerSalaries));
        }

        return workerSummaries;
    }

    private static IEnumerable<Worker> GetManagerWorkersLinqUseToList(IEnumerable<Worker> workers)
    {
        return workers.Where(x => x.workType == WorkType.Manager).ToList();
    }

    private static IEnumerable<Worker> GetRichWorkersLinqUseToList(IEnumerable<Worker> workers)
    {
        return workers.Where(x => x.salary > halfSalary).ToList();
    }

    [Benchmark]
    public List<WorkerSummary> IEnumerableFilterLinqtTest()
    {
        var managerWorkers = GetManagerWorkersLinq(workers);
        var richManagerWorkers = GetRichWorkersLinq(managerWorkers);

        decimal sumOfRichManagerSalaries = richManagerWorkers.Sum(x => x.salary);

        var workerSummaries = new List<WorkerSummary>(richManagerWorkers.Count());
        foreach (var richManagerWorker in richManagerWorkers)
        {
            workerSummaries.Add(new WorkerSummary(richManagerWorker, richManagerWorker.salary / sumOfRichManagerSalaries));
        }

        return workerSummaries;
    }

    private static IEnumerable<Worker> GetManagerWorkersLinq(IEnumerable<Worker> workers)
    {
        return workers.Where(x => x.workType == WorkType.Manager);
    }

    private static IEnumerable<Worker> GetRichWorkersLinq(IEnumerable<Worker> workers)
    {
        return workers.Where(x => x.salary > halfSalary);
    }

    [Benchmark]
    public List<WorkerSummary> IEnumerableFilterLinqTwoPassForeachTest()
    {
        var managerWorkers = GetManagerWorkersLinq(workers);
        var richManagerWorkers = GetRichWorkersLinq(managerWorkers);

        decimal sumOfRichManagerSalaries = 0;
        int count = 0;
        foreach (var item in richManagerWorkers)
        {
            sumOfRichManagerSalaries += item.salary;
            count++;
        }

        var workerSummaries = new List<WorkerSummary>(count);
        foreach (var richManagerWorker in richManagerWorkers)
        {
            workerSummaries.Add(new WorkerSummary(richManagerWorker, richManagerWorker.salary / sumOfRichManagerSalaries));
        }

        return workerSummaries;
    }

    [Benchmark]
    public List<WorkerSummary> IntegratedFilterTwoListOnePassTest()
    {
        List<Worker> richManagerWorkers = new List<Worker>();
        decimal sumOfRichManagerSalaries = 0;
        foreach (var worker in workers)
        {
            if (worker.workType == WorkType.Manager && worker.salary > halfSalary)
            {
                sumOfRichManagerSalaries += worker.salary;
                richManagerWorkers.Add(worker);
            }
        }

        var workerSummaries = new List<WorkerSummary>(richManagerWorkers.Count);
        foreach (var richManagerWorker in richManagerWorkers)
        {
            workerSummaries.Add(new WorkerSummary(richManagerWorker, richManagerWorker.salary / sumOfRichManagerSalaries));
        }

        return workerSummaries;
    }

    [Benchmark]
    public List<WorkerSummary> IntegratedFilterOneListTwoPassesTest()
    {
        decimal sumOfRichManagerSalaries = 0;
        int count = 0;
        foreach (var worker in workers)
        {
            if (worker.workType == WorkType.Manager && worker.salary > halfSalary)
            {
                sumOfRichManagerSalaries += worker.salary;
                count++;
            }
        }

        var workerSummaries = new List<WorkerSummary>(count);
        foreach (var worker in workers)
        {
            if (worker.workType == WorkType.Manager && worker.salary > halfSalary)
            {
                workerSummaries.Add(new WorkerSummary(worker, worker.salary / sumOfRichManagerSalaries));
            }
        }

        return workerSummaries;
    }

    [Benchmark]
    public List<WorkerSummary> IntegratedFilterOneListOnePassTest()
    {
        decimal sumOfRichManagerSalaries = 0;
        int count = 0;
        Span<int> indecies = stackalloc int[workers.Count];
        for (int i = 0; i < workers.Count; i++)
        {
            var worker = workers[i];
            if (worker.workType == WorkType.Manager && worker.salary > halfSalary)
            {
                sumOfRichManagerSalaries += worker.salary;
                indecies[0] = i;
                count++;
            }
        }

        var workerSummaries = new List<WorkerSummary>(count);
        for (int i = 0; i < count; i++)
        {
            var index = indecies[i];
            var worker = workers[index];
            workerSummaries.Add(new WorkerSummary(worker, worker.salary / sumOfRichManagerSalaries));
        }

        return workerSummaries;
    }
}
