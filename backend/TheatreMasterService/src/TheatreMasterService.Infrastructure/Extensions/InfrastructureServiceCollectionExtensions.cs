using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TheatreMasterService.Application.Abstractions.Persistence;
using TheatreMasterService.Infrastructure.Gateways;
using TheatreMasterService.Infrastructure.Persistence.Context;
using TheatreMasterService.Infrastructure.Repositories;
using TheatreMasterService.Application.Abstractions.Services;

namespace TheatreMasterService.Infrastructure.Extensions
{
    /// <summary>
    /// Provides extension methods for registering persistence, repositories, HTTP gateways, and background workers.
    /// </summary>
    public static class InfrastructureServiceCollectionExtensions
    {
        /// <summary>
        /// Adds EF Core DbContext, repositories, typed HTTP clients, and background RabbitMQ workers.
        /// </summary>
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // Database Context
            services.AddDbContext<TheatreMasterDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("TheatreMasterServiceConnection")));

            // Repositories
            services.AddScoped<ITheatreRepository, TheatreRepository>();
            services.AddScoped<IScreenRepository, ScreenRepository>();
            services.AddScoped<IShowRepository, ShowRepository>();

            // HTTP Gateways for Inter-service Communication
            services.AddHttpClient<IMicroServiceGateway, MicroServiceGateway>();

            return services;
        }
    }
}