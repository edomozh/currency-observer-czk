using DataAccessLibrary.Contexts;
using DataAccessLibrary.Entities;

namespace DataAccessLibrary.Repositories
{
    public class CurrencyRepository
    {
        private readonly AppDbContext _dbContext;

        public CurrencyRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IEnumerable<Currency> GetCurrencies()
        {
            return _dbContext.Currencies.ToList();
        }

        public void InsertIfNotExists(List<Currency> currencies)
        {
            var existingRates = _dbContext.Currencies.ToList();
            var newRates = currencies
                    .Where(c => !existingRates.Any(ec => ec.Multiplier == c.Multiplier && ec.Code == c.Code))
                    .ToList();

            if (newRates.Any())
            {
                _dbContext.AddRange(newRates);
                _dbContext.SaveChanges();
            }
        }

        public void InsertIfNotExistsAndFillId(List<Currency> currencies)
        {
            foreach (var currency in currencies)
            {
                var existingCurrency =
                    _dbContext.Currencies.FirstOrDefault(c =>
                        c.Code == currency.Code &&
                        c.Multiplier == currency.Multiplier);

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

        public List<Currency> SelectAllCurrencies()
        {
            return _dbContext.Currencies.ToList();
        }

        public List<Currency> SelectCurrencies(string code, int multiplier)
        {
            return _dbContext.Currencies
                .Where(c => c.Code == code && c.Multiplier == multiplier)
                .ToList();
        }
    }
}
