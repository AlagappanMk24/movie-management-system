using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using UserService.Application.Abstractions;
using UserService.Application.Services;

namespace UserService.Application.Extensions
{
    /// <summary>
    /// Provides extension methods for registering application-layer services and validators.
    /// </summary>
    public static class ApplicationServiceCollectionExtensions
    {
        /// <summary>
        /// Adds application-layer business services and FluentValidation validators to the dependency injection container.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <returns>The updated service collection.</returns>
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<IUserService, UserService.Application.Services.UserService>();
            services.AddScoped<IAuthService, AuthService>();

            services.AddValidatorsFromAssembly(typeof(ApplicationServiceCollectionExtensions).Assembly);

            return services;
        }
    }
}