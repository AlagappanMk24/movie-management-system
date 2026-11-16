using BookingService.Application.Abstractions.Messaging;
using BookingService.Application.DTOs.Events;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace BookingService.Infrastructure.Messaging
{
    /// <summary>
    /// RabbitMQ implementation of the message bus client for publishing domain events.
    /// </summary>
    public class MessageBusClient(IConfiguration configuration) : IMessageBusClient, IDisposable
    {
        private IConnection? _connection;
        private IChannel? _channel;
        private readonly IConfiguration _configuration = configuration;

        private async Task EnsureConnectionAsync()
        {
            if (_connection == null || !_connection.IsOpen)
            {
                var factory = new ConnectionFactory
                {
                    HostName = _configuration["RabbitMQHost"],
                    Port = int.Parse(_configuration["RabbitMQPort"] ?? "5672")
                };

                _connection = await factory.CreateConnectionAsync();
                _channel = await _connection.CreateChannelAsync();

                await _channel.ExchangeDeclareAsync(
                    exchange: "trigger",
                    type: ExchangeType.Fanout
                );
            }
        }

        /// <inheritdoc />
        public async Task PublishConfirmedBookingAsync(BookingConfirmed bookingConfirmed)
        {
            await EnsureConnectionAsync();

            var message = JsonSerializer.Serialize(bookingConfirmed);
            var body = Encoding.UTF8.GetBytes(message);

            await _channel!.BasicPublishAsync(
                exchange: "trigger",
                routingKey: string.Empty,
                body: body
            );

            Console.WriteLine($"RabbitMQ Sent: {message}");
        }

        /// <inheritdoc />
        public void Dispose()
        {
            _channel?.Dispose();
            _connection?.Dispose();
        }
    }
}