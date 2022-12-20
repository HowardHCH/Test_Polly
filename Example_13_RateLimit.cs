using Polly;
using Polly.RateLimit;

public static class Example_13_RateLimit
{
    private static RateLimitPolicy policy = Policy
        // Allow up to {?} executions {time period} with a burst of {?} executions.
        .RateLimit(numberOfExecutions:5, perTimeSpan: TimeSpan.FromSeconds(1), maxBurst: 3);

    private static void Act(int id)
    {
        try
        {
            policy.Execute(() => {Console.WriteLine($"Exceuting {id}"); Thread.Sleep(10000); });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"{id}:{ex.Message}");
        }
    }

    public static void TryIt()
    {
        List<Task> tasks = new();
        tasks.Add(Task.Factory.StartNew(() => { Act(1); })); Thread.Sleep(10);//Thread.Sleep(10);
        tasks.Add(Task.Factory.StartNew(() => { Act(2); })); Thread.Sleep(10);//Thread.Sleep(100);
        tasks.Add(Task.Factory.StartNew(() => { Act(3); })); Thread.Sleep(10);//Thread.Sleep(100);
        tasks.Add(Task.Factory.StartNew(() => { Act(4); })); Thread.Sleep(10);//Thread.Sleep(200);
        tasks.Add(Task.Factory.StartNew(() => { Act(5); })); Thread.Sleep(10);//Thread.Sleep(100);
        tasks.Add(Task.Factory.StartNew(() => { Act(6); })); 

        Task.WaitAll(tasks.ToArray());
    }
}
