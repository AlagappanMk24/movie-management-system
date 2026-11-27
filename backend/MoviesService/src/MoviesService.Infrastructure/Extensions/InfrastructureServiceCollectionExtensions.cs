using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MoviesService.Application.Abstractions.Persistence;
using MoviesService.Application.Helpers;
using MoviesService.Infrastructure.Persistence.Context;
using MoviesService.Infrastructure.Persistence.Repositories;
using MoviesService.Infrastructure.Services;

namespace MoviesService.Infrastructure.Extensions
{
    /// <summary>
    /// Provides extension methods for registering infrastructure-layer persistence, repositories, gateways, and helpers.
    /// </summary>
    public static class InfrastructureServiceCollectionExtensions
    {
        /// <summary>
        /// Adds EF Core DbContext, Dapper context, repositories, HTTP gateways, memory cache, and file storage helpers.
        /// </summary>
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<MoviesDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("MovieServiceConnection")));

            services.AddScoped<DapperContext>();
            services.AddScoped<IMoviesRepository, MoviesRepository>();
            services.AddScoped<ImageHelper>();

            services.AddHttpClient<IMicroServiceGateway, MicroServiceGateway>();
            services.AddMemoryCache();

            return services;
        }
    }
}