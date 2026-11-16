using BookingService.Application.Abstractions.Services;
using BookingService.Application.Services;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace BookingService.Application.Extensions
{
    /// <summary>
    /// Provides extension methods for registering application-layer business services and validators.
    /// </summary>
    public static class ApplicationServiceCollectionExtensions
    {
        /// <summary>
        /// Registers application services, analytics, and validators from the assembly.
        /// </summary>
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<IBookingService, Services.BookingService>();
            services.AddScoped<IBookingAnalyticsService, BookingAnalyticsService>();
            services.AddValidatorsFromAssembly(typeof(ApplicationServiceCollectionExtensions).Assembly);

            return services;
        }
    }
}