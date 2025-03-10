using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using RivertyTasks.Data;
using RivertyTasks.Models;

namespace RivertyTasks
{
    public static class TestDataCreator
    {
        private static string fixerKey;
        private static string baseUrl;

        public static void Initialize(IConfiguration configuration)
        {
            fixerKey = configuration["FixerApiKey"] ?? throw new InvalidOperationException("FixerApiKey is missing from configuration.");
            baseUrl = configuration["FixerBaseUrl"] ?? throw new InvalidOperationException("FixerBaseUrl is missing from configuration.");
        }
        public static async Task DataCreatorAsync(IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ExchangeRateDbContext>();

            if (context.ExchangeRates.Any()) return;

            HttpClient client = new HttpClient();

            for (int i = 0; i < 7; i++)
            {
                DateTime date = DateTime.UtcNow.AddDays(-i);
                string url = $"{baseUrl}{date:yyyy-MM-dd}?access_key={fixerKey}&base=EUR";

                try
                {
                    HttpResponseMessage response = await client.GetAsync(url);
                    response.EnsureSuccessStatusCode();
                    string responseBody = await response.Content.ReadAsStringAsync();
                    var json = JObject.Parse(responseBody);

                    if (json["success"]?.Value<bool>() == false) continue;

                    foreach (var rate in json["rates"])
                    {
                        string targetCurrency = rate.Path.Split('.').Last(); 
                        float exchangeRateValue = rate.First.Value<float>();
                        string baseCurrency = json["base"].ToString();
                        DateTime exchangeDate = DateTime.Parse(json["date"].ToString());

                        var exchangeRate = new ExchangeRate
                        {
                            BaseCurrency = baseCurrency,
                            TargetCurrency = targetCurrency,
                            ExchangeRateValue = exchangeRateValue,
                            ExchangeDate = exchangeDate
                        };

                        context.ExchangeRates.Add(exchangeRate);
                    }

                    await context.SaveChangesAsync();
                    Console.WriteLine($"Data for {date:yyyy-MM-dd} saved.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error seeding data for {date:yyyy-MM-dd}: {ex.Message}");
                }
            }
        }
    }
}