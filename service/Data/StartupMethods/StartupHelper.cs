using Data.IRepository;
using Data.Repository;
using Data.Transaction;
using Microsoft.Extensions.DependencyInjection;

namespace Data.StartupMethods
{
    public class StartupHelper
    {
        public static void InjectDependency(IServiceCollection services)
        {
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<IDBTransaction, DBTransaction>();
        }

    }
}
