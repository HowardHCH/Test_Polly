using Polly;

public static class Example_04_WaitAndRetry
{
    public static void TryIt()
    {
        try
        {
            var policy = Policy
                .Handle<HttpRequestException>(ex => ex.StatusCode == System.Net.HttpStatusCode.BadRequest)
                // Retry a specified number of times, using a function to
                // calculate the duration to wait between retries based on
                // the current retry attempt (allows for exponential back-off)
                // In this case will wait for
                //  2 ^ 1 = 2 seconds then
                //  2 ^ 2 = 4 seconds then
                //  2 ^ 3 = 8 seconds then
                .WaitAndRetry(3, (retryAttemp)=> TimeSpan.FromSeconds(Math.Pow(2, retryAttemp)), onRetry: (ex, duration) =>
                    {
                        Console.WriteLine($"{DateTime.Now.ToString("mm:ss.fff")} retrying! Duration: {duration}");
                    });

                // or waiting a specified duration between each retry.
                // .WaitAndRetry(new[]
                //     {
                //         TimeSpan.FromSeconds(1),
                //         TimeSpan.FromSeconds(2),
                //         TimeSpan.FromSeconds(3)
                //     }
                //     , onRetry: (ex, duration) =>
                //     {
                //         Console.WriteLine($"{DateTime.Now.ToString("mm:ss.fff")} retrying! Duration: {duration}");
                //     });

            policy.Execute(() => new SomeApiRequest().Call_API1("400"));
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"{DateTime.Now.ToString("mm:ss.fff")} final result: {ex.StatusCode}");
        }
    }
}
