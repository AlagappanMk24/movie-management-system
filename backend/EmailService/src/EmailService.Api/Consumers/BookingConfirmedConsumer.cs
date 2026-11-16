using EmailService.Application.Abstractions.Messaging;
using EmailService.Application.DTOs.Events;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace EmailService.Api.Consumers
{
    /// <summary>
    /// Background worker service that consumes <see cref="BookingConfirmed"/> events from RabbitMQ.
    /// </summary>
    public class BookingConfirmedConsumer(
        IConfiguration configuration,
        IServiceScopeFactory serviceScopeFactory,
        ILogger<BookingConfirmedConsumer> logger) : BackgroundService
    {
        private readonly IConfiguration _configuration = configuration;
        private readonly IServiceScopeFactory _serviceScopeFactory = serviceScopeFactory;
        private readonly ILogger<BookingConfirmedConsumer> _logger = logger;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var factory = new ConnectionFactory
            {
                HostName = _configuration["RabbitMQHost"] ?? "localhost",
                Port = int.Parse(_configuration["RabbitMQPort"] ?? "5672")
            };

            var connection = await factory.CreateConnectionAsync(stoppingToken);
            var channel = await connection.CreateChannelAsync(cancellationToken: stoppingToken);

            await channel.ExchangeDeclareAsync(exchange: "trigger", type: ExchangeType.Fanout, cancellationToken: stoppingToken);
            var queueDeclareResult = await channel.QueueDeclareAsync(cancellationToken: stoppingToken);
            var queueName = queueDeclareResult.QueueName;

            await channel.QueueBindAsync(queue: queueName, exchange: "trigger", routingKey: string.Empty, cancellationToken: stoppingToken);

            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.ReceivedAsync += async (model, ea) =>
            {
                try
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    var bookingEvent = JsonSerializer.Deserialize<BookingConfirmed>(message);

                    if (bookingEvent != null)
                    {
                        using var scope = _serviceScopeFactory.CreateScope();
                        var emailProcessor = scope.ServiceProvider.GetRequiredService<IEmailProcessingService>();
                        await emailProcessor.ProcessBookingConfirmationAsync(bookingEvent);
                    }

                    await channel.BasicAckAsync(ea.DeliveryTag, false, stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing RabbitMQ message in EmailService.");
                }
            };

            await channel.BasicConsumeAsync(queue: queueName, autoAck: false, consumer: consumer, cancellationToken: stoppingToken);

            // Keep background service alive
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
    }
}