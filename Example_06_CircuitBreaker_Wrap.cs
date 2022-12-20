using System.Net;
using Polly;
using Polly.CircuitBreaker;

public static class Example_06_CircuitBreaker_Wrap
{

    private static CircuitBreakerPolicy policyCB = Policy
                .Handle<HttpRequestException>()
                .CircuitBreaker(2, TimeSpan.FromSeconds(5), (exception, timespan) => { Console.WriteLine($"onBreak, please wait {timespan}"); }, () => { Console.WriteLine("onReset"); });

    private static Policy policy = Policy
                .Handle<HttpRequestException>()
                .WaitAndRetry(3, retryAttemp=> TimeSpan.FromSeconds(Math.Pow(2,retryAttemp)), onRetry: (ex, duration) =>
                    {
                        Console.WriteLine($"{DateTime.Now.ToString("mm:ss.fff")} retrying! Duration: {duration}");
                    })
                .Wrap(policyCB);

    private static void Act(int cnt, string code)
    {
        try
        {
            Console.WriteLine($"==[{cnt}]==[{code}]==Circuit break status: {policyCB.CircuitState}==");
            var rlt = policy.Execute(() => new SomeApiRequest().Call_API1(code));
            Console.WriteLine(rlt.Content.ReadAsStringAsync().GetAwaiter().GetResult());
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"{DateTime.Now.ToString("mm:ss.fff")} final result: {ex.StatusCode}");
        }
        catch (BrokenCircuitException bex)
        {
            Console.WriteLine($"Circuit break status: {policyCB.CircuitState}, {bex.Message}");
            Thread.Sleep(2000);
        }
    }

    public static void TryIt()
    {
        int cnt = 0;
        Act(++cnt, "400");
        Act(++cnt, "200");
        Act(++cnt, "400");
        Act(++cnt, "400");
        Act(++cnt, "400");
    }
}
