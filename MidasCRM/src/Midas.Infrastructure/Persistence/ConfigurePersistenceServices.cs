using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Midas.Application.Common.Interfaces.Queries;
using Midas.Application.Common.Interfaces.Repositories;
using Midas.Infrastructure.Persistence.Queries;
using Midas.Infrastructure.Persistence.Repositories;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence
{
    public static class ConfigurePersistenceServices
    {
        public static void AddPersistenceServices(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            if (!string.IsNullOrEmpty(connectionString))
            {
                var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);
                dataSourceBuilder.EnableDynamicJson();
                var dataSource = dataSourceBuilder.Build();
                services.AddSingleton(dataSource);

                services.AddDbContext<ApplicationDbContext>(options => options
                    .UseNpgsql(
                        dataSource,
                        builder => builder.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName))
                    .UseSnakeCaseNamingConvention()
                    .ConfigureWarnings(w => w.Ignore(CoreEventId.ManyServiceProvidersCreatedWarning)));
            }
            else
            {
                services.AddDbContext<ApplicationDbContext>();
            }

            services.AddScoped<ApplicationDbContextInitialiser>();
            services.AddRepositories();
        }

        private static void AddRepositories(this IServiceCollection services)
        {
           services.AddScoped(typeof(IEntityRepository<>), typeof(EntityRepository<>));
           services.AddScoped(typeof(IGetQueries<,>), typeof(GetQueries<,>));
        }
    }
}
