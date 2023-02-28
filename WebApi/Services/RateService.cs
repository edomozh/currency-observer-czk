using DataAccessLibrary.Repositories;
using System.Globalization;
using WebApi.Models;

namespace WebApi.Services
{
    public class RateService
    {
        private readonly RateRepository _rateRepository;

        public RateService(RateRepository rateRepository)
        {
            _rateRepository = rateRepository;
        }

        public Rate GetRate(string currencyCode, string ddMMyyyyDate)
        {
            var date = DateTime.ParseExact(ddMMyyyyDate, "ddMMyyyy", CultureInfo.InvariantCulture);

            var rate = _rateRepository.SelectRate(currencyCode, date);

            return new Rate
            {
                CurrencyCode = currencyCode,
                Value = rate.Value / rate.Currency!.Multiplier
            };
        }
    }
}
