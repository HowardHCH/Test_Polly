public class SomeApiRequest
{
    HttpClient httpClient = new HttpClient();

    public HttpResponseMessage Call_API1(string responseCode)
    {
        httpClient.BaseAddress = new Uri("https://db01844f-ec14-47bc-ab90-9334a0ca07d8.mock.pstmn.io");

        // postman mock server測試使用
        httpClient.DefaultRequestHeaders.Add("x-mock-response-code", responseCode);

        var response = httpClient.GetAsync("/API_01?id=123").GetAwaiter().GetResult();

        if (responseCode != "200")
            response.EnsureSuccessStatusCode();

        return response;
    }

    public HttpResponseMessage Call_Timeout_API1(double timeout)
    {
        httpClient.BaseAddress = new Uri("https://db01844f-ec14-47bc-ab90-9334a0ca07d8.mock.pstmn.io");

        httpClient.Timeout = TimeSpan.FromMilliseconds(timeout);
        var response = httpClient.GetAsync("/API_01?id=123").GetAwaiter().GetResult();

        return response;
    }
}