using Microsoft.AspNetCore.Mvc;
using RivertyTasks.Services;
using RivertyTasks.Models;

namespace RivertyTasks.Controllers
{
    [ApiController]
    [Route("api/currency")]
    public class CurrencyController : ControllerBase
    {
        private readonly ExchangeRateService _exchangeRateService;
        private static readonly string fixerKey;
        private static readonly string baseURL;
        private static IConfiguration configuration;

        static CurrencyController()
        {
            configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            fixerKey = configuration["FixerApiKey"] ?? throw new InvalidOperationException("FixerApiKey is missing from configuration.");
            baseURL = configuration["FixerBaseUrl"] ?? throw new InvalidOperationException("FixerBaseUrl is missing from configuration.");
        }

        public CurrencyController(ExchangeRateService exchangeRateService)
        {
            _exchangeRateService = exchangeRateService;
        }

        [HttpGet("currencies")]
        public async Task<IActionResult> GetAllCurrencies()
        {
            string url = $"{baseURL}latest?access_key={fixerKey}";

            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    return StatusCode((int)response.StatusCode, "Failed to fetch currencies.");
                }

                var data = await response.Content.ReadFromJsonAsync<CurrencyInfo>();
                if (data == null || data.Rates == null)
                {
                    return BadRequest("Invalid response from API.");
                }

                var currencyList = data.Rates.Keys.ToList();
                return Ok(currencyList);
            }
        }

        [HttpGet("convert")]
        public async Task<IActionResult> ConvertCurrency([FromQuery] string from, [FromQuery] string to, [FromQuery] decimal amount, [FromQuery] DateTime? date)
        {
            if (string.IsNullOrEmpty(from) || string.IsNullOrEmpty(to) || amount <= 0)
            {
                return BadRequest("Invalid input");
            }

            decimal convertedAmount = await _exchangeRateService.ConvertCurrency(from, to, amount, date);
            return Ok(new { from, to, amount, convertedAmount });
        }

        [HttpGet("exchangeHistory")]
        public async Task<ActionResult<object>> GetExchangeRateHistory(
            [FromQuery] string baseCurrency,
            [FromQuery] string targetCurrency,
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate)
        {
            try
            {
                var rates = await _exchangeRateService.GetExchangeRatesAsync(baseCurrency.ToUpper(), targetCurrency.ToUpper(), startDate, endDate);

                if (!rates.Any())
                {
                    return NotFound("No exchange rates found for the given period.");
                }

                return Ok(rates);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
