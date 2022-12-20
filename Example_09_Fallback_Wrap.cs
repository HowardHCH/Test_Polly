using Polly;

public static class Example_09_Fallback_Wrap
{
    public static void TryIt()
    {
        var policy = Policy
            .Handle<HttpRequestException>(ex => ex.StatusCode == System.Net.HttpStatusCode.BadRequest)
            .WaitAndRetry(3, (retryAttemp)=> TimeSpan.FromSeconds(Math.Pow(2, retryAttemp))
                , onRetry: (excption,duration)=>{Console.WriteLine("Retry!");});

        var policyF = Policy
            .Handle<HttpRequestException>(ex => ex.StatusCode == System.Net.HttpStatusCode.BadRequest)
            .Fallback(() =>
                {
                    var rlt = new SomeApiRequest().Call_API1("200").Content.ReadAsStringAsync().GetAwaiter().GetResult();
                    Console.WriteLine($"fallback result: {rlt}");
                }
                , onFallback: (ex) => { Console.WriteLine($"onFallback, Call fallback API"); })
            .Wrap(policy);

        policyF.Execute(() => { new SomeApiRequest().Call_API1("400"); });
    }
}
