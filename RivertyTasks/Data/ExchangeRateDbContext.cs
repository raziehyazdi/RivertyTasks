using Microsoft.EntityFrameworkCore;
using RivertyTasks.Models;

namespace RivertyTasks.Data
{
    public class ExchangeRateDbContext : DbContext
    {
        public ExchangeRateDbContext(DbContextOptions<ExchangeRateDbContext> options)
            : base(options) { }

        public DbSet<ExchangeRate> ExchangeRates { get; set; }
    }
}