using EmailService.Api.Consumers;
using EmailService.Application.Models;

namespace EmailService.Api.Extensions
{
    /// <summary>
    /// Provides extension methods for registering API-level settings, controllers, and background workers.
    /// </summary>
    public static class ApiServiceCollectionExtensions
    {
        /// <summary>Configures email settings options, background RabbitMQ consumers, and controllers.</summary>
        public static IServiceCollection AddApiServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));
            services.AddHostedService<BookingConfirmedConsumer>();

            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

            return services;
        }
    }
}