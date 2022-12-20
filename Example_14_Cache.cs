using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using Bogus;
using Microsoft.Extensions.Caching.Memory;
using Polly;
using Polly.Caching;
using Polly.Caching.Memory;

public class FakedUser
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public int Gender { get; set; }
    public DateTime Birthday { get; set; }
    public int Weight { get; set; }
    public string? Email { get; set; }
    public IList<FakedUserContacts>? ContactInfos { get; set; }
}
public class FakedUserContacts
{
    public int Seq { get; set; }
    public int ContantType { get; set; }
    public string? ContactInfo { get; set; }
}

public static class Example_14_Cache
{
    private static MemoryCache memoryCache = new MemoryCache(new MemoryCacheOptions());
    private static MemoryCacheProvider memoryCacheProvider = new MemoryCacheProvider(memoryCache);
    // 此處方便測試觀察,設置cache 200毫秒,實際情況須依據需求調整
    private static CachePolicy cachePolicy = Policy.Cache(memoryCacheProvider, TimeSpan.FromMilliseconds(200));


    public enum ContantType { CellPhone, Office, Home };
    private static FakedUser GetFoo()
    {
        Console.WriteLine("cache expired, update content");

        // Fack data generate by Bogus
        int seq = 0;
        var userContact = new Faker<FakedUserContacts>()
            .RuleFor(_ => _.Seq, f => seq++)
            .RuleFor(_ => _.ContantType, f => (int)f.PickRandom<ContantType>())
            .RuleFor(_ => _.ContactInfo, f => f.Phone.PhoneNumber("###-###-######"));

        var user = new Faker<FakedUser>()
            .RuleFor(_ => _.Id, f => f.Random.Guid())
            .RuleFor(_ => _.Gender, f => f.Random.Number(0, 1))
            .RuleFor(_ => _.Name, (f, u) => f.Name.FullName((Bogus.DataSets.Name.Gender)u.Gender))
            .RuleFor(_ => _.Birthday, f => f.Date.Past(60).Date)
            .RuleFor(_ => _.Weight, f => f.Random.Number(30, 100))
            .RuleFor(_ => _.Email, (f, u) => f.Internet.ExampleEmail(u.Name))
            .RuleFor(_ => _.ContactInfos, f => userContact.Generate(f.Random.Number(1, 3)))
            .FinishWith((f, u) => { seq = 0; });

        return user.Generate();
    }

    public static IList<string> GetBar()
    {

        Console.WriteLine("cache expired, update content");
        var faker = new Faker("zh_TW");
        List<string> names = new();
        for (int x = 0; x < faker.Random.Number(1, 10); x++)
        {
            names.Add($"{faker.Name.LastName()} {faker.Name.FirstName()}");
        }
        return names;
    }

    public static void TryIt()
    {
        var options = new JsonSerializerOptions
        {
            // 避免中文字元被轉成 UCN (Unicode Character Name)
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.CjkUnifiedIdeographs),
            WriteIndented = true
        };

        var result = cachePolicy.Execute(context => GetFoo(), new Context("FooKey"));
        Console.WriteLine(JsonSerializer.Serialize(result, options));
        Thread.Sleep(100);
        result = cachePolicy.Execute(context => GetFoo(), new Context("FooKey"));
        Console.WriteLine(JsonSerializer.Serialize(result, options));
        Thread.Sleep(100);
        result = cachePolicy.Execute(context => GetFoo(), new Context("FooKey"));
        Console.WriteLine(JsonSerializer.Serialize(result, options));
    }
}
