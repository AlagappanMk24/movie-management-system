using EmailService.Application.Abstractions.Messaging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Shared.Contracts.Events;
using System.Text;
using System.Text.Json;

namespace EmailService.Api.Consumers
{
    /// <summary>
    /// Background worker that starts automatically with EmailService, listens to RabbitMQ,
    /// and sends confirmation emails whenever a booking is confirmed.
    /// </summary>
    public class BookingConfirmedConsumer(
        IConfiguration configuration,
        IServiceScopeFactory serviceScopeFactory,
        ILogger<BookingConfirmedConsumer> logger) : BackgroundService
    {
        private readonly IConfiguration _configuration = configuration;
        private readonly IServiceScopeFactory _serviceScopeFactory = serviceScopeFactory;
        private readonly ILogger<BookingConfirmedConsumer> _logger = logger;

        // Triggered once when EmailService starts up (and stopped when the app shuts down).
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Read RabbitMQ connection settings and open a TCP connection + channel.
            var factory = new ConnectionFactory
            {
                HostName = _configuration["RabbitMQHost"] ?? "localhost",
                Port = int.Parse(_configuration["RabbitMQPort"] ?? "5672")
            };

            var connection = await factory.CreateConnectionAsync(stoppingToken);
            var channel = await connection.CreateChannelAsync(cancellationToken: stoppingToken);

            // Ensure the "trigger" fanout exchange exists (idempotent: safe to run repeatedly).
            // Fanout broadcasts every published message to all queues bound to this exchange.
            await channel.ExchangeDeclareAsync(
                exchange: "trigger",
                durable: true,
                autoDelete: false,
                type: ExchangeType.Fanout,
                cancellationToken: stoppingToken
            );

            // Ensure the durable "booking.notification" queue exists so messages are not lost
            // even if EmailService is temporarily offline.
            var queueDeclareResult = await channel.QueueDeclareAsync(
                queue: "booking.notification",
                durable: true,
                exclusive: false,
                autoDelete: false,
                cancellationToken: stoppingToken);
            var queueName = queueDeclareResult.QueueName;

            // Bind the queue to the "trigger" exchange so incoming events route here.
            // Routing key is empty because fanout exchanges ignore routing keys.
            await channel.QueueBindAsync(
                queue: queueName,
                exchange: "trigger",
                routingKey: string.Empty,
                cancellationToken: stoppingToken
            );

            // Register the message handler (does NOT run yet; runs only when a message arrives).
            var consumer = new AsyncEventingBasicConsumer(channel);

            // Triggered every time RabbitMQ delivers a new message to "booking.notification".
            consumer.ReceivedAsync += async (model, ea) =>
            {
                try
                {
                    // Convert raw message bytes into JSON string and deserialize into BookingConfirmed.
                    byte[] body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    var bookingEvent = JsonSerializer.Deserialize<BookingConfirmed>(message);

                    if (bookingEvent != null)
                    {
                        // Create a fresh DI scope because BackgroundService is a Singleton,
                        // while IEmailProcessingService (and its DB/SMTP dependencies) is usually Scoped.
                        using var scope = _serviceScopeFactory.CreateScope();
                        var emailProcessor = scope.ServiceProvider.GetRequiredService<IEmailProcessingService>();

                        // Send the confirmation email / process notification for this booking.
                        await emailProcessor.ProcessBookingConfirmationAsync(bookingEvent);
                    }

                    // Tell RabbitMQ the message was processed successfully so it can remove it from the queue.
                    // Because autoAck is false, skipping BasicAckAsync means RabbitMQ keeps it unacked.
                    await channel.BasicAckAsync(ea.DeliveryTag, false, stoppingToken);
                }
                catch (Exception ex)
                {
                    // Triggered if deserialization, email sending, or ack fails.
                    // Note: Without Nack/Reject here, failed messages stay unacknowledged until consumer restart/redelivery.
                    _logger.LogError(ex, "Error processing RabbitMQ message in EmailService.");
                }
            };

            // Start actively listening for messages on "booking.notification" (autoAck: false = manual ack).
            await channel.BasicConsumeAsync(queue: queueName, autoAck: false, consumer: consumer, cancellationToken: stoppingToken);

            // Keep ExecuteAsync alive until EmailService stops (stoppingToken is cancelled).
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
    }
}