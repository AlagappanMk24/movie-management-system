using Microsoft.Extensions.DependencyInjection;
using TheatreMasterService.Application.Abstractions.Services;
using TheatreMasterService.Application.Services;

namespace TheatreMasterService.Application.Extensions
{
    /// <summary>
    /// Provides extension methods for registering application-layer business logic services.
    /// </summary>
    public static class ApplicationServiceCollectionExtensions
    {
        /// <summary>
        /// Registers theatre, screen, and show use-case business services.
        /// </summary>
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<ITheatreService, TheatreService>();
            services.AddScoped<IScreenService, ScreenService>();
            services.AddScoped<IShowService, ShowService>();

            return services;
        }
    }
}