
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;

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
        Console.Write("Enter target currency (e.g., EUR):");
        string fromCurrency = Console.ReadLine().ToUpper();


        Console.Write("Enter base currency (e.g., USD): ");
        string toCurrency = Console.ReadLine().ToUpper();

        Console.Write("Enter amount: ");
        if (!decimal.TryParse(Console.ReadLine(), out decimal amount) || amount <= 0)
        {
            Console.WriteLine("Enter a valid amount. ");
            return;
        }

        Console.Write("Enter a date (in YYYY-MM-DD format), or press Enter for the latest rate: ");
        string dateInput = Console.ReadLine();

        DateTime? exchangeDate = null;
        if (!string.IsNullOrWhiteSpace(dateInput) && DateTime.TryParse(dateInput, out DateTime parsedDate))
        {
            exchangeDate = parsedDate;
        }

        decimal convertedAmount = await ConvertCurrency(fromCurrency, toCurrency, amount, exchangeDate);

        if (convertedAmount != -1)
            Console.WriteLine($"Your converted amount is: {convertedAmount} {toCurrency}");
        else
            Console.WriteLine("Oops, something went wrong while fetching the exchange rate. Try again later.");
    }

    static async Task<decimal> ConvertCurrency(string fromCurrency, string toCurrency, decimal amount, DateTime? date)
    {
        using HttpClient client = new HttpClient();
        string url = date.HasValue
                ? $"{baseURL}{date.Value:yyyy-MM-dd}?access_key={fixerKey}&base=EUR&symbols={fromCurrency},{toCurrency}"
                : $"{baseURL}latest?access_key={fixerKey}";

        decimal fromRate;
        decimal toRate;
        try
        {
            HttpResponseMessage response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();

            string responseBody = await response.Content.ReadAsStringAsync();
            var rates = JObject.Parse(responseBody)["rates"];

            if (rates[fromCurrency] == null || rates[toCurrency] == null)
            {
                Console.WriteLine("One or both of the currencies you entered are not valid. Try again.");
                return -1;
            }
            fromRate = rates[fromCurrency].Value<decimal>();
            toRate = rates[toCurrency].Value<decimal>();
            return (amount / fromRate) * toRate;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Something went wrong: {ex.Message}. Please try again.");
            return -1;
        }
    }
}