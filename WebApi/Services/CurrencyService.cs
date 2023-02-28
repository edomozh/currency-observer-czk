using DataAccessLibrary.Repositories;
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
    }
}
