This project contains three different apps that work together to give you real-time currency conversion and exchange rate history.

What's Inside?
1. ConsoleAppCurrencyConverter
This is a simple console app where you can convert currencies. You enter an amount and choose the currencies you want to convert between, and the app will show you the converted value based on live exchange rates.
2. CurrencyExchangeHistory
This project stores and fetches past exchange rates. Want to know how the exchange rate between two currencies looked like last month? This app will fetch that data for you!
3. CurrencyExchangeWebAPI
A Web API that lets you interact with the currency conversion and exchange history features over HTTP. This project exposes a bunch of useful endpoints, like:
GET /api/currency/convert: Convert one currency to another.
GET /api/currency/currencies: Get a list of available currencies.
GET /api/currency/exchangeHistory: Get historical exchange rates between two currencies.
Before You Start
Prerequisites:
Before you run any of the projects, you’ll need to:

Install .NET SDK

Make sure you’ve got the .NET SDK installed. You can grab it from here.
Get an API Key for Fixer.io

To get real-time currency exchange data, you’ll need an API key from Fixer.io. It’s pretty easy to sign up and get your key.
Add Your API Key

All of the projects need to be set up with the API key and base URL for Fixer.io.
Open the appsettings.json in each project and add your Fixer API Key and Base URL.
Example appsettings.json:

json
Copy
Edit
{
    "FixerApiKey": "your-api-key-here",
    "FixerBaseUrl": "http://data.fixer.io/api/"
}
How to Run It
1. ConsoleAppCurrencyConverter
Go to the ConsoleAppCurrencyConverter project folder.
Open your terminal and run:
bash
Copy
Edit
dotnet run
Follow the prompts in the console to convert currencies. It’s simple and fun to use!
2. CurrencyExchangeHistory
Go to the CurrencyExchangeHistory project folder.
This project will automatically pull historical exchange rate data and store it in a database (or just in memory, depending on your setup).
3. CurrencyExchangeWebAPI
Head over to the CurrencyExchangeWebAPI project folder.
Run this API using:
bash
Copy
Edit
dotnet run
The API will start running locally, and you can make requests like:
GET /api/currency/convert
GET /api/currency/currencies
GET /api/currency/exchangeHistory


API Endpoints
Here are some useful endpoints from the Web API:

GET /api/currency/currencies
Get a list of all the currencies available for conversion.

GET /api/currency/convert?from={fromCurrency}&to={toCurrency}&amount={amount}
Convert an amount from one currency to another.

GET /api/currency/exchangeHistory?baseCurrency={fromCurrency}&targetCurrency={toCurrency}&startDate={startDate}&endDate={endDate}
Get the historical exchange rates between two currencies over a period of time.

What Technologies Are Used?
C# (.NET 6): All projects are built using the .NET 6 framework.
ASP.NET Core: Used for building the Web API.
Entity Framework Core: For managing databases in the CurrencyExchangeHistory project.
Fixer.io API: We rely on Fixer.io to fetch live and historical exchange rate data.
