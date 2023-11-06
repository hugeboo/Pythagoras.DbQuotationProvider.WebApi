using Microsoft.EntityFrameworkCore;
using Pythagoras.DbQuotationProvider.Persistence.Entities;

namespace Pythagoras.DbQuotationProvider.Persistence
{
    internal sealed class AppDbContext : DbContext
    {
        public DbSet<HQuotationTick> HQuotationTicks => Set<HQuotationTick>();

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<HQuotationTick>().HasKey(u => new { u.Ticker, u.Time });
        }
    }
}