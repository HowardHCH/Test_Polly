using Polly;

public static class Example_08_Fallback
{
    public static void TryIt()
    {
        var policy = Policy
            .Handle<HttpRequestException>(ex => ex.StatusCode == System.Net.HttpStatusCode.BadRequest)
            .Fallback(() =>
                {
                    var rlt = new SomeApiRequest().Call_API1("200").Content.ReadAsStringAsync().GetAwaiter().GetResult();
                    Console.WriteLine($"fallback result: {rlt}");
                }
                , onFallback: (ex) => { Console.WriteLine($"onFallback, Call fallback API"); });

        policy.Execute(() => { new SomeApiRequest().Call_API1("400"); });
    }
}
