using DataAccessLibrary.Contexts;
using DataAccessLibrary.Entities;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLibrary.Repositories
{
    public class CurrencyRepository
    {
        private readonly AppDbContext _dbContext;

        public CurrencyRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IEnumerable<Currency> GetCurrencies(DateTime date)
        {
            return from currency in _dbContext.Currencies
                   join rate in _dbContext.Rates on currency.Id equals rate.CurrencyId
                   where rate.Date == date
                   select currency;
        }

        public IEnumerable<Currency> GetCurrencies()
        {
            return _dbContext.Currencies.ToList();
        }

        public void InsertIfNotExistsOrFillId(List<Currency> currencies)
        {
            foreach (var currency in currencies)
            {
                var existingCurrency = _dbContext.Currencies
                    .FirstOrDefault(c => c.Code == currency.Code && c.Multiplier == currency.Multiplier);

                if (existingCurrency != null)
                {
                    currency.Id = existingCurrency.Id;
                }
                else
                {
                    _dbContext.Currencies.Add(currency);
                    _dbContext.SaveChanges();
                }
            }
        }
    }
}
