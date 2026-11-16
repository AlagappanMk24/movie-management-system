using EmailService.Application.Abstractions.Messaging;
using EmailService.Application.Abstractions.Services;
using EmailService.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace EmailService.Application.Extensions
{
    /// <summary>
    /// Provides extension methods for registering application-layer business processors.
    /// </summary>
    public static class ApplicationServiceCollectionExtensions
    {
        /// <summary>Registers application email processing services.</summary>
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<IEmailProcessingService, EmailProcessingService>();
            return services;
        }
    }
}