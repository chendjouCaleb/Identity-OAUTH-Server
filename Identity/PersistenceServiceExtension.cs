using Everest.Identity.Models;
using Everest.Identity.Core.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace Everest.Identity
{
    public static class PersistenceServiceExtension
    {
        public static void AddRepositories(this IServiceCollection services)
        {
            services.AddTransient<IRepository<Account, string>, Repository<Account, string>>();
            services.AddTransient<IRepository<Connection, long>, Repository<Connection, long>>();
            services.AddTransient<IRepository<Client, string>, Repository<Client, string>>();
            services.AddTransient<IRepository<Authorization, long>, Repository<Authorization, long>>();
        }
    }
}
