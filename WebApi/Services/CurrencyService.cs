using DataAccessLibrary.Repositories;
using System.Globalization;
using WebApi.Models;

namespace WebApi.Services
{
    public class CurrencyService
    {
        private readonly CurrencyRepository _repository;

        public CurrencyService(CurrencyRepository repository)
        {
            _repository = repository;
        }

        public IEnumerable<Currency> GetCurrencies()
        {
            return _repository.GetCurrencies()
                .Select(c => new Currency { Code = c.Code });
        }

        public IEnumerable<Currency> GetCurrencies(string ddMMyyyyDate)
        {
            var date = DateTime.ParseExact(ddMMyyyyDate, "ddMMyyyy", CultureInfo.InvariantCulture);

            return _repository.GetCurrencies(date)
                .Select(c => new Currency { Code = c.Code });
        }
    }
}
