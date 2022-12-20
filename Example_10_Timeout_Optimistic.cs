using Polly;
using Polly.Timeout;

public static class Example_10_Timeout_Optimistic
{
    public static void TryIt()
    {
        var policy = Policy
            .Timeout(TimeSpan.FromMilliseconds(100), TimeoutStrategy.Pessimistic, onTimeout: (context, timespan, task) =>
            {
                Console.WriteLine($"{context.PolicyKey} at {context.OperationKey}: execution timed out after {timespan.TotalSeconds} seconds.");
            });

        policy.Execute(()=>{new SomeApiRequest().Call_Timeout_API1(200);});
    }
}
