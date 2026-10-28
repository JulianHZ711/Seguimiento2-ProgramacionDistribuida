using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Access.Application.Contracts.Persistence;
using Access.Application.Contracts.Repositories;
using Access.Persistence.Repositories;
using Access.Persistence.Seeds;
using Access.Persistence.Seeds.AccessPoints;
using Access.Persistence.Seeds.Areas;
using Access.Persistence.UnitOfWorks;
using System;
using System.Collections.Generic;
using System.Text;

namespace Access.Persistence
{
    public static class PersistenceServicesRegistry
    {
        public static IServiceCollection AddPersistenceServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<DataContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("MyConnection"));
            });

            services.AddScoped<IUnitOfWork, EfCoreUnitOfWork>();
            services.AddScoped<IAreasRepository, AreasRepository>();
            services.AddScoped<IEmployeesRepository, EmployeesRepository>();
            services.AddScoped<ICredentialsRepository, CredentialsRepository>();
            services.AddScoped<IAccessPointsRepository, AccessPointsRepository>();
            services.AddScoped<IAccessLogsRepository, AccessLogsRepository>();

            // Seeders
            services.AddScoped<IDataSeeder, AreaSeeder>();
            services.AddScoped<IDataSeeder, AccessPointSeeder>();

            return services;
        }
    }
}
