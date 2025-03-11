This project contains three different apps that work together to give you real-time currency conversion and exchange rate history.

## **What's Inside?**

1. **ConsoleAppCurrencyConverter**  
   This is a console app where you can convert currencies. You enter an amount and choose the currencies you want to convert between, and the app will show you the converted value based on live exchange rates.

2. **CurrencyExchangeHistory**  
   The CurrencyExchangeHistory project fetches and stores daily exchange rates using an in-memory database for fast data retrieval. This approach eliminates the need for an external database, simplifying the setup and enhancing performance. It is designed to be run as a scheduled job, which will be configured in Azure to regularly update exchange rates.

4. **CurrencyExchangeWebAPI**  
   The CurrencyExchangeWebAPI project provides a Web API that allows interaction with currency conversion and exchange history features. It exposes useful endpoints such as:
   GET /api/currency/convert: Convert one currency to another.
   GET /api/currency/currencies: Get a list of available currencies.
   GET /api/currency/exchangeHistory: Get historical exchange rates between two currencies.
   Additionally, the API includes a chart to visually represent exchange rate changes for two currencies over a selected period. To support this, the project fetches data for the past 7 days and stores it in memory for quick access, allowing the chart to display the historical exchange rate changes.


   **Swagger Documentation**
   Swagger has been integrated into the Web API for easy exploration of the available endpoints. When you run the project, you can access the Swagger UI at http://localhost:5000/swagger

---

### **Before You Start**

#### Prerequisites

Before you run any of the projects, you’ll need to:

1. **Install .NET SDK**  
   From here [here](https://dotnet.microsoft.com/download).

2. **Get an API Key for Fixer.io**  
   You’ll need an API key from [Fixer.io](https://fixer.io/). 

#### **Add Your API Key**  
All of the projects need to be set up with the API key and base URL for Fixer.io.  
Open the `appsettings.json` in each project and add your Fixer API Key and Base URL.  

**Example `appsettings.json`:**
```json
{
    "FixerApiKey": "api-key",
    "FixerBaseUrl": "http://data.fixer.io/api/"
}
```
### **API Endpoints**

Here are some useful endpoints from the Web API:

- **GET /api/currency/currencies**  
  Get a list of all the currencies available for conversion.

- **GET /api/currency/convert?from={fromCurrency}&to={toCurrency}&amount={amount}**  
  Convert an amount from one currency to another.

- **GET /api/currency/exchangeHistory?baseCurrency={fromCurrency}&targetCurrency={toCurrency}&startDate={startDate}&endDate={endDate}**  
  Get the historical exchange rates between two currencies over a period of time.
