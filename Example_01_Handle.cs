using Polly;

public static class Example_01_Handle
{
    public static void TryIt()
    {
        try
        {
            var policy = Policy
                .Handle<HttpRequestException>(ex => ex.StatusCode == System.Net.HttpStatusCode.BadRequest)
                .Retry(onRetry: (ex, retrycount) =>
                    { 
                        Console.WriteLine($"{DateTime.Now.ToString("mm:ss.fff")} retrying! Count:{retrycount}"); 
                    });

            policy.Execute(() => new SomeApiRequest().Call_API1("400"));
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"{DateTime.Now.ToString("mm:ss.fff")} final result: {ex.StatusCode}");
        }
    }
}
