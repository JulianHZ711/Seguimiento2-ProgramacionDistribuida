using Microsoft.Extensions.DependencyInjection;
using System.Collections;
using System.Collections.Generic;

namespace Access.Persistence.Seeds
{
    public static class DataBaseSeeder
    {
        public static async Task SeedAsync(IServiceProvider services, CancellationToken cancellationToken = default)
        {
            using IServiceScope scope = services.CreateScope();

            IEnumerable<IDataSeeder> seeders = scope.ServiceProvider.GetServices<IDataSeeder>()
                                                                    .OrderBy(s => s.Order);

            foreach(IDataSeeder seeder in seeders)
            {
                await seeder.SeedAsync(cancellationToken);
            }
        }
    }
}
