using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using DataAccessLibrary.Entities;

namespace DataAccessLibrary.Contexts
{
    public class AppDbContext : DbContext
    {
        public DbSet<Currency> Currencies { get; set; }

        public DbSet<Rate> Rates { get; set; }

        private readonly IConfiguration _configuration;

        public AppDbContext(IConfiguration configuration)
        {
            _configuration = configuration;

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_configuration.GetConnectionString("CurrencyObserverCZK"));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Currency>()
                .HasAlternateKey(c => new { c.Code, c.Multiplier });

            modelBuilder.Entity<Rate>()
                .HasAlternateKey(c => new { c.CurrencyId, c.Date });

        }
    }
}
