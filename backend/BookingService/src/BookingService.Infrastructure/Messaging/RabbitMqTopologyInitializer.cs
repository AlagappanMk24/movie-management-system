using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace BookingService.Infrastructure.Messaging
{
    public sealed class RabbitMqTopologyInitializer(
        IConfiguration configuration,
        ILogger<RabbitMqTopologyInitializer> logger) : IHostedService
    {
        private const string ExchangeName = "trigger";

        private static readonly string[] Queues =
        {
            "booking.notification",
            "booking.analytics",
            "booking.email"
        };

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                var host = configuration["RabbitMQ:Host"] ?? "localhost";
                var port = int.TryParse(configuration["RabbitMQ:Port"], out var p) ? p : 5672;

                var factory = new ConnectionFactory
                {
                    HostName = host,
                    Port = port,
                    UserName = configuration["RabbitMQ:Username"] ?? "guest",
                    Password = configuration["RabbitMQ:Password"] ?? "guest",
                    AutomaticRecoveryEnabled = true,
                };

                await using var connection = await factory.CreateConnectionAsync(cancellationToken);
                await using var channel = await connection.CreateChannelAsync(cancellationToken: cancellationToken);

                // 🔥 FORCE DELETE: Wipe out any existing "trigger" exchange
                try
                {
                    await channel.ExchangeDeleteAsync(ExchangeName, ifUnused: false, cancellationToken: cancellationToken);
                    logger.LogInformation("Deleted existing stale exchange '{Exchange}'.", ExchangeName);
                }
                catch (Exception ex)
                {
                    // Ignore if it doesn't exist
                    logger.LogDebug(ex, "No existing exchange '{Exchange}' to delete.", ExchangeName);
                }

                // 1. Re-create Exchange with DURABLE = TRUE
                await channel.ExchangeDeclareAsync(
                    exchange: ExchangeName,
                    type: ExchangeType.Fanout,
                    durable: true,       // <--- This will now work because we deleted the old one
                    autoDelete: false,
                    cancellationToken: cancellationToken);

                logger.LogInformation("Exchange '{Exchange}' declared (durable=true).", ExchangeName);

                // 2. Queues + bindings
                foreach (var q in Queues)
                {
                    await channel.QueueDeclareAsync(
                        queue: q,
                        durable: true,
                        exclusive: false,
                        autoDelete: false,
                        arguments: null,
                        cancellationToken: cancellationToken);

                    await channel.QueueBindAsync(
                        queue: q,
                        exchange: ExchangeName,
                        routingKey: string.Empty,
                        arguments: null,
                        cancellationToken: cancellationToken);

                    logger.LogInformation("Queue '{Queue}' bound to exchange '{Exchange}'.", q, ExchangeName);
                }
            }
            catch (Exception ex)
            {
                logger.LogCritical(ex, "Failed to initialize RabbitMQ topology.");
            }
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}