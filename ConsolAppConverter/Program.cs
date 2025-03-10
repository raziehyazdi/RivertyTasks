using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace ConsolAppConverter
{
    class Program
    {
        private const string API_KEY = "your_fixer_api_key"; // Replace with your API key
        private const string BASE_URL = "https://data.fixer.io/api/latest";

        static async Task Main()
        {
            Console.Write("Enter base currency (e.g., USD): ");
            string fromCurrency = Console.ReadLine().ToUpper();

            Console.Write("Enter target currency (e.g., EUR): ");
            string toCurrency = Console.ReadLine().ToUpper();

            Console.Write("Enter amount: ");
            if (!decimal.TryParse(Console.ReadLine(), out decimal amount) || amount <= 0)
            {
                Console.WriteLine("Invalid amount.");
                return;
            }

            decimal convertedAmount = await ConvertCurrency(fromCurrency, toCurrency, amount);

            if (convertedAmount != -1)
                Console.WriteLine($"Converted amount: {convertedAmount} {toCurrency}");
            else
                Console.WriteLine("Error fetching exchange rate.");
        }

        static async Task<decimal> ConvertCurrency(string fromCurrency, string toCurrency, decimal amount)
        {
            using HttpClient client = new HttpClient();
            string url = $"{BASE_URL}?access_key={API_KEY}";

            try
            {
                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();
                var rates = JObject.Parse(responseBody)["rates"];

                if (rates[fromCurrency] == null || rates[toCurrency] == null)
                {
                    Console.WriteLine("Invalid currency codes.");
                    return -1;
                }

                decimal fromRate = rates[fromCurrency].Value<decimal>();
                decimal toRate = rates[toCurrency].Value<decimal>();

                return (amount / fromRate) * toRate;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return -1;
            }
        }
    }
}
