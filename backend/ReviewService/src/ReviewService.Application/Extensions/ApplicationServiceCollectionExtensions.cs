using Microsoft.Extensions.DependencyInjection;
using ReviewService.Application.Abstractions.Services;

namespace ReviewService.Application.Extensions
{
    /// <summary>
    /// Provides extension methods for registering application-layer business logic services.
    /// </summary>
    public static class ApplicationServiceCollectionExtensions
    {
        /// <summary>
        /// Registers review business services.
        /// </summary>
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<IReviewService, Services.ReviewService>();

            return services;
        }
    }
}