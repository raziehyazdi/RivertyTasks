This project contains three different apps that work together to give you real-time currency conversion and exchange rate history.

## **What's Inside?**

1. **ConsoleAppCurrencyConverter**  
   This is a console app where you can convert currencies. You enter an amount and choose the currencies you want to convert between, and the app will show you the converted value based on live exchange rates.

2. **CurrencyExchangeHistory**  
   This project stores and fetches past exchange rates. Want to know how the exchange rate between two currencies looked like last month? This app will fetch that data for you!

3. **CurrencyExchangeWebAPI**  
   A Web API that lets you interact with the currency conversion and exchange history features over HTTP. This project exposes a bunch of useful endpoints, like:
   - **GET /api/currency/convert:** Convert one currency to another.
   - **GET /api/currency/currencies:** Get a list of available currencies.
   - **GET /api/currency/exchangeHistory:** Get historical exchange rates between two currencies.

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
    "FixerApiKey": "your-api-key-here",
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
