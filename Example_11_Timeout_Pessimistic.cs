using Polly;
using Polly.Timeout;

public static class Example_11_Timeout_Pessimistic
{
    public static void TryIt()
    {
        var policy = Policy
            .Timeout(TimeSpan.FromMilliseconds(100), TimeoutStrategy.Pessimistic, onTimeout: (context, timespan, task) =>
            {
                Console.WriteLine($"{context.PolicyKey} at {context.OperationKey}: execution timed out after {timespan.TotalSeconds} seconds.");
            });

        policy.Execute(() => { Thread.Sleep(200); });
    }
}
