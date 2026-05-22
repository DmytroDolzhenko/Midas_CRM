using Application.Common.Interfaces;
using CloudinaryDotNet;
using Infrastructure.Persistence.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Midas.Application.Common.Interfaces;
using Midas.Application.Common.Interfaces.Queries;
using Midas.Application.Common.Interfaces.Repositories;
using Midas.Infrastructure.Persistence.Queries;
using Midas.Infrastructure.Persistence.Repositories;
using Midas.Infrastructure.Persistence.Services;
using Midas.Infrastructure.Persistence.Services.NovaPoshta;
using Npgsql;

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

            services.AddHttpContextAccessor();

            services.AddScoped<ApplicationDbContextInitialiser>();
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
            services.AddScoped<IUniqCodeGenerator, UniqCodeGenerator>();
            services.Configure<Midas.Infrastructure.Persistence.Services.CloudinaryConfiguration>(configuration.GetSection("Cloudinary"));
            services.AddScoped<IFileService, CloudinaryService>();
            services.AddScoped<IEncryptionService, AesEncryptionService>();
            services.AddScoped<IIntegrationStateService, HmacIntegrationStateService>();
            services.Configure<AiAssistantSettings>(configuration.GetSection("AiSettings"));
            services.AddHttpClient<IAiAssistantService, AiAssistantService>();

            services.AddScoped<NovaPoshtaSyncService>();
            services.AddHostedService<NovaPoshtaSyncWorker>();
            services.AddScoped<OrderTrackingService>();
            services.AddHostedService<NovaPoshtaOrderTrackingWorker>();

            services.AddScoped<IAuthorizationHandler, NotDeletedHandler>();

            services.AddRepositories();
            services.AddIntegrationProviders(configuration);
        }

        private static void AddRepositories(this IServiceCollection services)
        {
            services.AddScoped(typeof(IEntityRepository<>), typeof(EntityRepository<>));
            services.AddScoped(typeof(IGetQueries<,>), typeof(GetQueries<,>));

            services.AddScoped<IOrderQueries, OrderQueries>();
            services.AddScoped<IProductVariantQueries, ProductVariantQueries>();
            services.AddScoped<ICustomerQueries, CustomerQueries>();
            services.AddScoped<IProductCategoryQueries, ProductCategoryQueries>();
            services.AddScoped<IUserQueries, UserQueries>();


            services.AddHttpClient<INovaPoshtaClient, NovaPoshtaClient>(client =>
            {
                client.BaseAddress = new Uri("https://api.novaposhta.ua/v2.0/json/");
                client.Timeout = TimeSpan.FromMinutes(5);
            });

            services.AddScoped<IApplicationDbContext>(provider =>
                provider.GetRequiredService<ApplicationDbContext>());
        }

        private static void AddIntegrationProviders(this IServiceCollection services, IConfiguration configuration)
        {
            var configuredProviders = configuration.GetSection("Integration:Providers").Get<List<OAuthProviderSettings>>()
                ?? [];

            foreach (var providerSettings in configuredProviders.Where(x => x.Enabled))
            {
                services.AddScoped<IIntegrationProvider>(sp =>
                    new GenericOAuthIntegrationProvider(
                        new HttpClient(),
                        providerSettings));
            }
        }
    }
}
