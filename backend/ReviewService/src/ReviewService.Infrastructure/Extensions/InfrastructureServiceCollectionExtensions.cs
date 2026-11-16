using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ReviewService.Application.Abstractions.Persistence;
using ReviewService.Infrastructure.Persistence.Context;
using ReviewService.Infrastructure.Repositories;

namespace ReviewService.Infrastructure.Extensions
{
    /// <summary>
    /// Provides extension methods for registering database persistence and data repositories.
    /// </summary>
    public static class InfrastructureServiceCollectionExtensions
    {
        /// <summary>
        /// Adds EF Core DbContext and review repositories.
        /// </summary>
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ReviewDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("ReviewServiceConnection")));

            services.AddScoped<IReviewRepository, ReviewRepository>();

            return services;
        }
    }
}