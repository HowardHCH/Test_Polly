using Polly;

public static class Example_02_HandleResult
{
    public static void TryIt()
    {
        var policy = Policy
            .HandleResult<HttpResponseMessage>(rlt => rlt.Content.ReadAsStringAsync().GetAwaiter().GetResult().Contains("message worth retrying"))
            .Retry(3, (rlt, retrycount) =>
                {
                    Console.WriteLine($"{DateTime.Now.ToString("mm:ss.fff")} retrying! Count: {retrycount}");
                });

        var rlt = policy.Execute(() => new SomeApiRequest().Call_API1("200"));
        Console.WriteLine($"{DateTime.Now.ToString("mm:ss.fff")} final result: {rlt.Content.ReadAsStringAsync().GetAwaiter().GetResult()}");
    }
}
