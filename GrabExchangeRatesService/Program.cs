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
        private static AppDbContext? appDbContext;
        private static CurrencyRepository? currencyRepository;
        private static RateRepository? rateRepository;

        private static void Initialize()
        {
            configuration = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json")
                        .Build();

            appDbContext = new AppDbContext(configuration);

            currencyRepository = new CurrencyRepository(appDbContext);
            rateRepository = new RateRepository(appDbContext);
        }

        static void Main(string[] args)
        {
            Initialize();

            var availableYears = Enumerable.Range(1991, DateTime.Now.Year - 1990);

            // get a last date from db
            // var latestRate = rateRepository.SelectLatestDate();

            var latestRate = DateTime.Now.AddDays(-1);

            if (latestRate.Date.CompareTo(DateTime.Now.Date) == 0)
            {
                // log
                return;
            }

            var missingYears = availableYears.Where(y => y >= latestRate.Date.Year).ToArray();

            // get all data for missing dates
            var validCsvLists = GetRatesCsvByYears(missingYears);

            // parse currencies and save
            var currencies = GetCurrenciesFromHeader(validCsvLists);
            currencyRepository!.InsertIfNotExists(currencies);

            // parse rates and save
            var rates = GetRates(validCsvLists, currencies);
            // rateRepository.InsertIfNotExists(rates);

            // dispose all

        }


        private static List<Rate> GetRates(List<List<string>> validCsvLists, List<Currency> currencies)
        {
            var rates = new List<Rate>();

            foreach (var csv in validCsvLists)
            {
                var headers = csv.First()
                    .Split('|')
                    .Select(c => c == "Date" ? (0, "Date") : ExtractMultipaier(c))
                    .Select(c => currencies.Find(cur => cur.Multiplier == c.Item1 && cur.Code == c.Item2))
                    .ToList();

                var devidedByDateRates = csv
                    .Where(r => !r.StartsWith("Data"))
                    .Select(r => r.Split('|'));

                foreach (var oneDayRates in devidedByDateRates)
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

        private static List<Currency> GetCurrenciesFromHeader(List<List<string>> validCsvLists)
        {
            var headers = validCsvLists
                .Select(csv => csv.First().Split('|'));

            var currencies = headers.Where(h => h.Equals("Data"));

            return new List<Currency>();
        }

        public static List<List<string>> GetRatesCsvByYears(int[] years)
        {
            var result = new List<List<string>>();

            foreach (var year in years)
            {
                var rates = GetRatesByUrl(year);

                var lists = DevideByConcreteHeader(rates);

                result.AddRange(lists);
            }

            return result;
        }

        private static List<List<string>> DevideByConcreteHeader(string[] csvPage)
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




        private static (short multiplayer, string code) ExtractMultipaier(string header)
        {
            string[] parts = header.Split(' ');

            if (parts.Length != 2)
                throw new ArgumentException("Invalid currency string format.", nameof(header));

            if (!short.TryParse(parts[0], out short amount))
                throw new ArgumentException("Invalid currency amount value.", nameof(header));

            string currencyCode = parts[1];
            return (amount, currencyCode);
        }

        private static string GetConfigValue(string path)
        {
            if (configuration is null)
                throw new NullReferenceException(nameof(configuration));

            return configuration.GetValue<string>(path) ??
                throw new NullReferenceException($"There is no config by path: {path}.");

        }

        public static string[] GetRatesByUrl(int year)
        {
            var url = GetConfigValue("ExternalUrls:GetCZKRate");

            // Create a new HttpClient instance
            using var httpClient = new HttpClient();

            // Download the CSV data as a string
            var csvData = httpClient.GetStringAsync(url + year).GetAwaiter().GetResult();

            return csvData.Split(new[] { '\r', '\n' });
        }
    }
}
