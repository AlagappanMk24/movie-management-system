using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using System.Text;
using System.Text.Json;

namespace UserService.Infrastructure.Messaging
{
    /// <summary>
    /// Background service that consumes messages from RabbitMQ with built-in connection retry resilience.
    /// </summary>
    public class RabbitMQSubscriber(IConfiguration configuration) : BackgroundService
    {
        #region Configuration
        private readonly IConfiguration _configuration = configuration;
        private IConnection? _connection;
        private IChannel? _channel;
        #endregion

        /// <summary>
        /// Executes the background service to connect to RabbitMQ with retry logic and listen for messages.
        /// </summary>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var host = _configuration["RabbitMQHost"] ?? "localhost";
            var portString = _configuration["RabbitMQPort"] ?? "5672";
            var port = int.Parse(portString);

            var factory = new ConnectionFactory
            {
                HostName = host,
                Port = port
            };

            // Retry loop to handle startup race conditions where RabbitMQ is still booting up
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _connection = await factory.CreateConnectionAsync(stoppingToken);
                    _channel = await _connection.CreateChannelAsync(cancellationToken: stoppingToken);

                    break; // Connection successful, exit retry loop
                }
                catch (BrokerUnreachableException)
                {
                    Console.WriteLine("RabbitMQ is not reachable yet. Retrying in 5 seconds...");
                    await Task.Delay(5000, stoppingToken);
                }
            }

            if (_channel == null || _connection == null) return;

            await _channel.ExchangeDeclareAsync(
                exchange: "trigger",
                type: ExchangeType.Fanout,
                cancellationToken: stoppingToken
            );

            var queue = await _channel.QueueDeclareAsync(cancellationToken: stoppingToken);

            await _channel.QueueBindAsync(
                queue: queue.QueueName,
                exchange: "trigger",
                routingKey: "",
                cancellationToken: stoppingToken
            );

            var consumer = new AsyncEventingBasicConsumer(_channel);

            consumer.ReceivedAsync += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var json = Encoding.UTF8.GetString(body);

                var booking = JsonSerializer.Deserialize<BookingConfirmed>(json);

                Console.WriteLine("Booking Event Received in UserService");
                Console.WriteLine($"BookingId : {booking?.BookingId}");
                Console.WriteLine($"Movie     : {booking?.Title}");
                Console.WriteLine($"Show Time : {booking?.ShowTime}");
                Console.WriteLine($"Seats     : {booking?.SeatCount}");

                await Task.CompletedTask;
            };

            await _channel.BasicConsumeAsync(
                queue: queue.QueueName,
                autoAck: true,
                consumer: consumer,
                cancellationToken: stoppingToken
            );

            await Task.Delay(Timeout.Infinite, stoppingToken);
        }

        /// <summary>
        /// Stops the background service and closes RabbitMQ connections safely.
        /// </summary>
        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            if (_channel != null) await _channel.CloseAsync(cancellationToken);
            if (_connection != null) await _connection.CloseAsync(cancellationToken);
            await base.StopAsync(cancellationToken);
        }
    }
}