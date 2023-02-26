using DataAccessLibrary.Contexts;
using DataAccessLibrary.Entities;

namespace DataAccessLibrary.Repositories
{
    public class RateRepository
    {
        private readonly AppDbContext _dbContext;

        public RateRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Rate SelectRate(string currencyCode, DateTime date)
        {
            var currencyIds = _dbContext.Currencies
                .Where(c => c.Code == currencyCode)
                .Select(c => c.Id)
                .ToList();

            var rate = _dbContext.Rates
                .Where(r => currencyIds.Contains(r.CurrencyId) && r.Date <= date)
                .OrderByDescending(r => r.Date)
                .FirstOrDefault();

            return rate;
        }

        public DateTime SelectLatestDate()
        {
            var rate = _dbContext.Rates
                .OrderByDescending(r => r.Date)
                .FirstOrDefault();

            return rate?.Date ?? DateTime.MinValue;
        }

        public void InsertIfNotExists(List<Rate> rates)
        {
            foreach (var rate in rates)
            {
                var existingRate = _dbContext.Rates
                    .Where(r => r.CurrencyId == rate.CurrencyId && r.Date == rate.Date)
                    .FirstOrDefault();

                if (existingRate != null)
                {
                    rate.Id = existingRate.Id;
                }
                else
                {
                    _dbContext.Rates.Add(rate);
                    _dbContext.SaveChanges();
                }
            }
        }
    }
}
