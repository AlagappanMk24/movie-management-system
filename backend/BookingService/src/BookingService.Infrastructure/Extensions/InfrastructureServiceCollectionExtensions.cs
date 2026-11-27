using BookingService.Application.Abstractions.Caching;
using BookingService.Application.Abstractions.Documents;
using BookingService.Application.Abstractions.Persistence;
using BookingService.Infrastructure.Caching;
using BookingService.Infrastructure.Documents;
using BookingService.Infrastructure.Messaging;
using BookingService.Infrastructure.Persistence.Context;
using BookingService.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using QuestPDF.Infrastructure;
using BookingService.Application.Abstractions.Messaging;
using BookingService.Application.Abstractions.Services;
using BookingService.Infrastructure.Persistence.Repositories;

namespace BookingService.Infrastructure.Extensions
{
    /// <summary>
    /// Provides extension methods for registering persistence, caching, messaging, and document generation infrastructure.
    /// </summary>
    public static class InfrastructureServiceCollectionExtensions
    {
        /// <summary>
        /// Adds EF Core, Dapper, Redis, Repositories, Message Bus, Gateways, and PDF/Excel document processors.
        /// </summary>
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // QuestPDF Community License Setup
            QuestPDF.Settings.License = LicenseType.Community;

            // Database Contexts
            services.AddDbContext<BookingServiceDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("BookingServiceConnection")));
            services.AddScoped<DapperContext>();

            // Redis Distributed Cache Setup
            var redisConnection = configuration.GetConnectionString("RedisURL");
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = redisConnection;
                options.InstanceName = "BookingServiceInstance";
            });

            // Repositories & Gateways
            services.AddScoped<IBookingRepository, BookingRepository>();
            services.AddScoped<IBookingSeatRepository, BookingSeatRepository>();

            // Typed HttpClient registration automatically registers the interface and handles client pooling
            services.AddHttpClient<IMicroServiceGateway, MicroServiceGateway>();

            // Caching & Document Services
            services.AddScoped<ICacheService, RedisCacheService>();
            services.AddScoped<IBookingExcelService, BookingExcelService>();
            services.AddScoped<ITicketPdfService, TicketPdfService>();

            // Messaging (RabbitMQ Singleton)
            services.AddSingleton<IMessageBusClient, MessageBusClient>();
            services.AddHostedService<RabbitMqTopologyInitializer>();

            return services;
        }
    }
}