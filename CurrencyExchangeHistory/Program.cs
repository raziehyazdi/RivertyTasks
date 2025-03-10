using Newtonsoft.Json.Linq;
using DotNetEnv;
using Microsoft.Extensions.Configuration;

class Program
{
    private static readonly string fixerKey;
    private static readonly string baseURL;
    private static IConfiguration configuration;

    static Program()
    {
        configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())  
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)  
            .Build();
        fixerKey = configuration["FixerApiKey"] ?? throw new InvalidOperationException("FixerApiKey is missing from configuration.");
        baseURL = configuration["FixerBaseUrl"] ?? throw new InvalidOperationException("FixerBaseUrl is missing from configuration.");

    }

    static async Task Main()
    {
        await FetchAndStoreExchangeRates();
    }

    static async Task FetchAndStoreExchangeRates()
    {
        using HttpClient client = new HttpClient();
        string url = $"{baseURL}latest?access_key={fixerKey}";

        try
        {
            HttpResponseMessage response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();
            var json = JObject.Parse(responseBody);

            if (json["success"].Value<bool>() == false)
            {
                Console.WriteLine("Error fetching exchange rates.");
                return;
            }

            using var db = new ExchangeRateDbContext();

            foreach (var rate in json["rates"])
            {
                string baseCurrency = json["base"].ToString();
                string targetCurrency = rate.Path.Split('.').Last();
                float exchangeRateValue = rate.First.Value<float>();
                DateTime exchangeDate = DateTime.Parse(json["date"].ToString());

                var existingRate = db.ExchangeRates
                    .FirstOrDefault(r => r.BaseCurrency == baseCurrency &&
                             r.TargetCurrency == targetCurrency &&
                             r.ExchangeDate == exchangeDate);

                if (existingRate != null)
                {
                    existingRate.ExchangeRateValue = exchangeRateValue;
                    existingRate.ExchangeDate = DateTime.UtcNow;
                }
                else
                {
                    var exchangeRateEntry = new ExchangeRate
                    {
                        BaseCurrency = baseCurrency,
                        TargetCurrency = targetCurrency,
                        ExchangeRateValue = (float)exchangeRateValue,
                        ExchangeDate = DateTime.UtcNow
                    };

                    db.ExchangeRates.Add(exchangeRateEntry);
                }
            }

            await db.SaveChangesAsync();
            Console.WriteLine("Exchange rates updated successfully.");
            DisplayStoredExchangeRates();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    static void DisplayStoredExchangeRates()
    {
        using var db = new ExchangeRateDbContext();
        var exchangeRates = db.ExchangeRates
            .OrderBy(r => r.ExchangeDate)
            .ToList();

        Console.WriteLine("\nStored Exchange Rates:");
        foreach (var rate in exchangeRates)
        {
            Console.WriteLine($"Base: {rate.BaseCurrency}, Target: {rate.TargetCurrency}, Rate: {rate.ExchangeRateValue}, Date: {rate.ExchangeDate}");
        }
    }
}