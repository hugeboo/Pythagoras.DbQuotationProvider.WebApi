using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Pythagoras.DbQuotationProvider.Persistence.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pythagoras.DbQuotationProvider.Persistence
{
    internal sealed class AppRepository : IAppRepository
    {
        private readonly AppDbContext _ctx;

        public AppRepository(AppDbContext ctx)
        {
            _ctx = ctx ?? throw new ArgumentNullException(nameof(ctx));
        }

        public async Task<HQuotationTick[]> GetLastTicks(DateTime beginTime, int intervalSec)
        {
            if (intervalSec <= 0) throw new ArgumentException("'intervalSec' must be greater than zero", nameof(intervalSec));
            var endTime = beginTime.AddSeconds(intervalSec);
            return await _ctx.HQuotationTicks
                .Where(q => q.Time >= beginTime && q.Time < endTime)
                .OrderBy(q => q.Time)
                .ToArrayAsync();
        }
    }

    public interface IAppRepository
    {
        Task<HQuotationTick[]> GetLastTicks(DateTime beginTime, int intervalSec);
    }

    public static class AppExtensions
    {
        public static IServiceCollection AddRepository(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<AppDbContext>(options => options
               .UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));
            services.AddScoped<IAppRepository, AppRepository>();
            return services;
        }
    }
}
