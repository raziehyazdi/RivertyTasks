using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using RivertyTasks.Data;
using RivertyTasks.Models;

namespace RivertyTasks.Services
{
    public class ExchangeRateService
    {
        private readonly HttpClient _httpClient;
        private readonly ExchangeRateDbContext _dbContext;
        private static readonly string fixerKey;
        private static readonly string baseURL;
        private static IConfiguration configuration;

        static ExchangeRateService()
        {
            configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();
            fixerKey = configuration["FixerApiKey"] ?? throw new InvalidOperationException("FixerApiKey is missing from configuration.");
            baseURL = configuration["FixerBaseUrl"] ?? throw new InvalidOperationException("FixerBaseUrl is missing from configuration.");
        }

        public ExchangeRateService(HttpClient httpClient, ExchangeRateDbContext dbContext)
        {
            _httpClient = httpClient;
            _dbContext = dbContext;
        }

        public async Task<decimal> ConvertCurrency(string fromCurrency, string toCurrency, decimal amount, DateTime? date)
        {
            
            string url = date.HasValue
                ? $"{baseURL}{date.Value:yyyy-MM-dd}?access_key={fixerKey}&base=EUR&symbols={fromCurrency},{toCurrency}"
                : $"{baseURL}latest?access_key={fixerKey}";
            HttpResponseMessage response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            string responseBody = await response.Content.ReadAsStringAsync();
            var rates = JObject.Parse(responseBody)["rates"]; 

            decimal fromRate = rates[fromCurrency].Value<decimal>();
            decimal toRate = rates[toCurrency].Value<decimal>();

            return (amount / fromRate) * toRate;
        }

        public async Task<List<ExchangeRate>> GetExchangeRatesAsync(string baseCurrency, string targetCurrency, DateTime startDate, DateTime endDate)
        {
            if (startDate > endDate)
            {
                throw new ArgumentException("Start date must be before end date.");
            }
            var eurToBaseCurrencyRate = await _dbContext.ExchangeRates
                .Where(r => r.BaseCurrency == "EUR" && r.TargetCurrency == baseCurrency &&
                       r.ExchangeDate >= startDate && r.ExchangeDate <= endDate)
                .OrderBy(r => r.ExchangeDate)
                .FirstOrDefaultAsync();

            var rates = await _dbContext.ExchangeRates
                 .Where(r => r.BaseCurrency == "EUR" &&
                    r.TargetCurrency == targetCurrency &&
                    r.ExchangeDate >= startDate &&
                    r.ExchangeDate <= endDate)
                 .OrderBy(r => r.ExchangeDate)
                 .ToListAsync();
            foreach (var rate in rates)
            {
                rate.ExchangeRateValue = rate.ExchangeRateValue / eurToBaseCurrencyRate.ExchangeRateValue;
                rate.ExchangeDate.ToShortDateString();
                rate.BaseCurrency = baseCurrency;
            }

            return rates;
        }
    }
}
