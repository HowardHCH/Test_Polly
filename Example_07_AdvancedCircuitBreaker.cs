using System.Net;
using Polly;
using Polly.CircuitBreaker;

public static class Example_07_AdvancedCircuitBreaker
{
    private static CircuitBreakerPolicy policy = Policy
                .Handle<HttpRequestException>()
                .AdvancedCircuitBreaker(
                    failureThreshold: 0.5, // Break on >=50% actions result in handled exceptions...
                    samplingDuration: TimeSpan.FromSeconds(10), // ... over any 10 second period
                    minimumThroughput: 8, // ... provided at least 8 actions in the 10 second period.
                    durationOfBreak: TimeSpan.FromSeconds(30), // Break for 30 seconds.
                    onBreak: (exception, timespan) => { Console.WriteLine($"onBreak, please wait {timespan}"); }, 
                    onReset: () => { Console.WriteLine("onReset"); }
                );

    private static void Act(int cnt, string code)
    {
        try
        {
            var rlt = policy.Execute(() => new SomeApiRequest().Call_API1(code));
            Console.WriteLine($"{DateTime.Now.ToString("mm:ss.fff")} result: {rlt.Content.ReadAsStringAsync().GetAwaiter().GetResult()}");
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"{DateTime.Now.ToString("mm:ss.fff")} final result: {ex.StatusCode}");
        }
        catch (BrokenCircuitException bex)
        {
            Console.WriteLine($"{DateTime.Now.ToString("mm:ss.fff")} Circuit bread status: {policy.CircuitState}, {bex.Message}");
        }
    }

    public static void TryIt()
    {
        int cnt = 0;
        Act(++cnt,"400");
        Act(++cnt,"200");
        Act(++cnt,"400");
        Act(++cnt,"400");
        Act(++cnt,"400");
        Act(++cnt,"400");
        Act(++cnt,"400"); 
        Act(++cnt,"400"); // circuit break
        Act(++cnt,"400"); // circuit break status : open
    }
}
