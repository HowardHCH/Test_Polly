using System.Net;
using Polly;
using Polly.CircuitBreaker;

public static class Example_05_CircuitBreaker
{
    private static CircuitBreakerPolicy policy = Policy
                .Handle<HttpRequestException>()
                .CircuitBreaker(3, TimeSpan.FromSeconds(60), 
                    onBreak: (exception, timespan) => { Console.WriteLine($"onBreak, please wait {timespan}"); }, 
                    onReset: () => { Console.WriteLine("onReset"); });

    private static void Act(int cnt)
    {
        try
        {
            var rlt = policy.Execute(() => new SomeApiRequest().Call_API1("400"));
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
        Act(++cnt);
        Act(++cnt);
        Act(++cnt);
        Act(++cnt);
    }
}
