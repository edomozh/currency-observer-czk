using DataAccessLibrary.Contexts;
using DataAccessLibrary.Entities;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLibrary.Repositories
{
    public class RateRepository
    {
        private readonly AppDbContext _dbContext;

        public RateRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
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
            var existingRates = _dbContext.Rates.ToList();
            var newRates = rates
                    .Where(c => !existingRates.Any(ec => ec.Date == c.Date && ec.CurrencyId == c.CurrencyId))
                    .ToList();

            if (newRates.Any())
            {
                _dbContext.AddRange(newRates);
                _dbContext.SaveChanges();
            }
        }

        public Rate SelectRate(string currencyCode, DateTime date)
        {
            var rate = _dbContext.Rates
                    .Include(r => r.Currency)
                    .Where(r => r.Date == date && r.Currency!.Code == currencyCode)
                    .First();

            return rate;
        }
    }
}
