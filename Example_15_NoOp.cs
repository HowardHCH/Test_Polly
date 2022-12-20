using System.Text.Json;
using Bogus;
using Microsoft.Extensions.Caching.Memory;
using Polly;
using Polly.Caching;
using Polly.Caching.Memory;

public static class Example_15_NoOp
{
    public static void TryIt()
    {
        // Define a policy which will simply cause delegates passed for execution to be executed 'as is'.
        // This is useful for stubbing-out Polly in unit tests,
        // or in application situations where your code architecture might expect a policy
        // but you simply want to pass the execution through without policy intervention.
        var policy = Policy.NoOp();

        var rlt = policy.Execute(() => new SomeApiRequest().Call_API1("200"));
        Console.WriteLine($"{rlt.Content.ReadAsStringAsync().GetAwaiter().GetResult()}");
    }
}
