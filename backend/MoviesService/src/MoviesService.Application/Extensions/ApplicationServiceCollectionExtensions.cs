using Microsoft.Extensions.DependencyInjection;
using MoviesService.Application.Abstractions;
using FluentValidation;

namespace MoviesService.Application.Extensions
{
    /// <summary>
    /// Provides extension methods for registering application-layer services and validators.
    /// </summary>
    public static class ApplicationServiceCollectionExtensions
    {
        /// <summary>
        /// Registers application services and validators from the assembly.
        /// </summary>
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<IMoviesService, MoviesService.Application.Services.MoviesService>();
            services.AddValidatorsFromAssembly(typeof(ApplicationServiceCollectionExtensions).Assembly);

            return services;
        }
    }
}