using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UserService.Application.Abstractions;
using UserService.Application.Abstractions.Persistence;
using UserService.Infrastructure.Messaging;
using UserService.Infrastructure.Persistence.Context;
using UserService.Infrastructure.Repositories;
using UserService.Infrastructure.Services;
using UserService.Infrastructure.Services.Security;

namespace UserService.Infrastructure.Extensions
{
    /// <summary>
    /// Provides extension methods for registering infrastructure-layer persistence, messaging, and external gateways.
    /// </summary>
    public static class InfrastructureServiceCollectionExtensions
    {
        /// <summary>
        /// Adds database contexts, Dapper infrastructure, token services, repositories, HTTP gateways, and background consumers to the DI container.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="configuration">The application configuration instance.</param>
        /// <returns>The updated service collection.</returns>
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<UserDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("UserServiceConnection")));

            services.AddScoped<DapperContext>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IUserRepository, UserRepository>();

            services.AddHttpClient<IMicroServiceGateway, MicroServiceGateway>();

            services.AddHostedService<RabbitMQSubscriber>();

            services.AddMemoryCache();

            return services;
        }
    }
}