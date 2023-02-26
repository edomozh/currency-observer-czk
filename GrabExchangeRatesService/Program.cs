using DataAccessLibrary.Contexts;
using DataAccessLibrary.Entities;
using DataAccessLibrary.Repositories;
using Microsoft.Extensions.Configuration;
using System.Globalization;

namespace GrabExchangeRatesService
{
    class Program
    {
        private static IConfigurationRoot? configuration;

        static void Main(string[] args)
        {
            configuration = new ConfigurationBuilder()
                       .SetBasePath(Directory.GetCurrentDirectory())
                       .AddJsonFile("appsettings.json")
                       .Build();

            using var appDbContext = new AppDbContext(configuration);
            var currencyRepository = new CurrencyRepository(appDbContext);
            var rateRepository = new RateRepository(appDbContext);

            // get a last date from db
            var latestRate = rateRepository.SelectLatestDate();

            if (latestRate.Date.CompareTo(DateTime.Now.Date) == 0)
            {
                // log
                return;
            }

            var availableYears = Enumerable.Range(1991, DateTime.Now.Year - 1990);
            var missingYears = availableYears.Where(y => y >= latestRate.Date.Year).ToArray();

            // get all data for missing dates
            var validCsvLists = GetRatesCsvByYears(missingYears);

            // parse currencies and save
            var currencies = GetCurrenciesFromCsvHeaders(validCsvLists);
            currencyRepository.InsertIfNotExists(currencies);

            // parse rates and save
            var rates = GetRates(validCsvLists, currencies);
            rateRepository.InsertIfNotExists(rates);
        }

        private static List<Rate> GetRates(List<List<string>> validCsvLists, List<Currency> currencies)
        {
            var rates = new List<Rate>();

            foreach (var csv in validCsvLists)
            {
                var headers = csv.First()
                    .Split('|')
                    .Where(h => !h.Equals("Date"))
                    .Select(c => StringToCurrency(c))
                    .Select(c => currencies.Single(cur => cur.Multiplier == c.Multiplier && cur.Code == c.Code))
                    .ToList();

                var ratesDevidedByDate = csv
                    .Where(r => !r.StartsWith("Date"))
                    .Select(r => r.Split('|'));

                foreach (var oneDayRates in ratesDevidedByDate)
                {
                    var date = DateTime.ParseExact(oneDayRates[0], "dd.MM.yyyy", CultureInfo.InvariantCulture);

                    for (var i = 1; i < headers.Count; i++)
                    {
                        rates.Add(new Rate
                        {
                            CurrencyId = headers[i].Id,
                            Value = Convert.ToDouble(oneDayRates[i]),
                            Date = date,
                        });
                    }
                }
            }

            return rates;
        }

        private static List<Currency> GetCurrenciesFromCsvHeaders(List<List<string>> validCsvLists)
        {
            var headers = validCsvLists
                .Select(csv => csv.First().Split('|').Where(h => !h.Equals("Date")));

            var uniqueCurrencies = MergeArrays(headers);

            var currencies = new List<Currency>();

            foreach (var currencyStr in uniqueCurrencies)
            {
                if (currencyStr.Equals("Date")) continue;

                var currency = StringToCurrency(currencyStr);

                currencies.Add(currency);
            }

            return currencies;
        }

        public static string[] MergeArrays(IEnumerable<IEnumerable<string>> arrays)
        {
            var uniqueValues = new HashSet<string>();

            foreach (var array in arrays)
                foreach (var value in array)
                    uniqueValues.Add(value);


            return uniqueValues.ToArray();
        }


        public static List<List<string>> GetRatesCsvByYears(int[] years)
        {
            var result = new List<List<string>>();

            foreach (var year in years)
            {
                var rates = GetCsvByUrl(year);

                var lists = DevideCsvByHeaders(rates);

                result.AddRange(lists);
            }

            return result;
        }

        private static List<List<string>> DevideCsvByHeaders(string[] csvPage)
        {
            var result = new List<List<string>>();

            var temp = new List<string>();

            foreach (string row in csvPage)
            {
                if (row.StartsWith("Date"))
                {
                    if (temp.Count > 1)
                    {
                        result.Add(temp);
                    }

                    temp = new List<string>();
                }

                temp.Add(row);
            }

            result.Add(temp);

            return result;
        }

        private static Currency StringToCurrency(string header)
        {
            string[] parts = header.Split(' ');

            if (parts.Length != 2)
                throw new NullReferenceException("Invalid currency string format.");

            if (!int.TryParse(parts[0], out int amount))
                throw new NullReferenceException("Invalid currency amount value.");

            if (parts[1].Length > 3)
                throw new NullReferenceException("Invalid currency code.");

            return new Currency { Multiplier = amount, Code = parts[1] };
        }

        private static string GetConfigValue(string path)
        {
            if (configuration is null)
                throw new NullReferenceException(nameof(configuration));

            return configuration.GetValue<string>(path) ??
                throw new NullReferenceException($"There is no config by path: {path}.");
        }

        public static string[] GetCsvByUrl(int year)
        {
            var url = GetConfigValue("ExternalUrls:GetCZKRate");

            using var httpClient = new HttpClient();

            var csvData = httpClient.GetStringAsync(url + year).GetAwaiter().GetResult();

            return csvData.Split(new[] { '\r', '\n' });
        }
    }
}
