using Polly;

public static class Example_03_Handle_COMBINED
{
    public static void TryIt()
    {
        var policy = Policy
            .Handle<HttpRequestException>(ex => ex.StatusCode == System.Net.HttpStatusCode.BadRequest)
            .OrResult<HttpResponseMessage>(rlt => rlt.Content.ReadAsStringAsync().GetAwaiter().GetResult().Contains("message worth retrying"))
            .Retry(3, (rlt, retrycount) =>
                {
                    Console.WriteLine($"{DateTime.Now.ToString("mm:ss.fff")} retrying! Count: {retrycount}");
                });

        var rlt = policy.Execute(() => new SomeApiRequest().Call_API1("400"));
        Console.WriteLine($"{DateTime.Now.ToString("mm:ss.fff")} final result: {rlt.Content.ReadAsStringAsync().GetAwaiter().GetResult()}");
    }
}
