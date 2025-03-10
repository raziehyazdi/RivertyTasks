using Microsoft.EntityFrameworkCore;

public class ExchangeRateDbContext : DbContext
{
    public DbSet<ExchangeRate> ExchangeRates { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseInMemoryDatabase("RivertyExchangeInMemoryDB");
}