using Polly;
using Polly.Bulkhead;
using Polly.Timeout;

public static class Example_12_Bulkhead
{
    private static BulkheadPolicy policy = Policy
            // Restrict executions through the policy to a maximum of {?} concurrent actions,
            // with up to {?} actions waiting for an execution slot in the bulkhead if all slots are taken.
            .Bulkhead(maxParallelization: 2, maxQueuingActions: 0, onBulkheadRejected: context =>
            {
                Console.WriteLine($"Reject:{context.PolicyKey}");
            });

    private static void Act(int id)
    {
        try
        {
            Console.WriteLine($"Exceuting {id}, Bulkhead Available Count: {policy.BulkheadAvailableCount}, Queue Available Count: {policy.QueueAvailableCount}");
            policy.Execute(() => { Thread.Sleep(10000); });
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
    public static void TryIt()
    {
        List<Task> tasks = new();
        tasks.Add(Task.Factory.StartNew(() => { Act(1); }));
        Thread.Sleep(500);
        tasks.Add(Task.Factory.StartNew(() => { Act(2); }));
        Thread.Sleep(500);
        tasks.Add(Task.Factory.StartNew(() => { Act(3); }));

        Task.WaitAll(tasks.ToArray());
    }
}
