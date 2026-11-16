using EmailService.Application.Abstractions.Services;
using EmailService.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http;

namespace EmailService.Infrastructure.Extensions
{
    /// <summary>
    /// Provides extension methods for registering infrastructure services and HTTP gateways.
    /// </summary>
    public static class InfrastructureServiceCollectionExtensions
    {
        /// <summary>Adds SMTP email senders and typed microservice HTTP gateways.</summary>
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IEmailSender, EmailSender>();
            services.AddHttpClient<IMicroServiceGateway, MicroServiceGateway>();

            return services;
        }
    }
}