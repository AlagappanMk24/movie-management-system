using BookingService.Application.Abstractions.Messaging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using Shared.Contracts.Events;
using System.Text;
using System.Text.Json;

namespace BookingService.Infrastructure.Messaging
{
    /// <summary>
    /// RabbitMQ publisher. Registered as SINGLETON in DI.
    /// </summary>
    public sealed class MessageBusClient(IConfiguration configuration, ILogger<MessageBusClient> logger) : IMessageBusClient, IAsyncDisposable
    {
        private const string ExchangeName = "trigger";

        private readonly IConfiguration _configuration = configuration;
        private readonly ILogger<MessageBusClient> _logger = logger;
        private readonly SemaphoreSlim _initLock = new(1, 1);

        private IConnection? _connection;
        private IChannel? _channel;

        private async Task<IChannel> GetChannelAsync()
        {
            if (_channel is { IsOpen: true })
                return _channel;

            await _initLock.WaitAsync();
            try
            {
                if (_channel is { IsOpen: true })
                    return _channel;

                var host = _configuration["RabbitMQ:Host"]
                    ?? throw new InvalidOperationException("RabbitMQ:Host is not configured.");
                var port = int.TryParse(_configuration["RabbitMQ:Port"], out var p) ? p : 5672;
                var user = _configuration["RabbitMQ:Username"] ?? "guest";
                var pass = _configuration["RabbitMQ:Password"] ?? "guest";

                var factory = new ConnectionFactory
                {
                    HostName = host,
                    Port = port,
                    UserName = user,
                    Password = pass,
                    AutomaticRecoveryEnabled = true,
                };

                _connection = await factory.CreateConnectionAsync();
                _channel = await _connection.CreateChannelAsync();

                await _channel.ExchangeDeclareAsync(
                    exchange: ExchangeName,
                    type: ExchangeType.Fanout,
                    durable: true,
                    autoDelete: false);

                _logger.LogInformation("RabbitMQ channel established, exchange '{Exchange}' declared.", ExchangeName);
                return _channel;
            }
            finally
            {
                _initLock.Release();
            }
        }

        /// <inheritdoc />
        public async Task PublishConfirmedBookingAsync(BookingConfirmed bookingConfirmed)
        {
            var channel = await GetChannelAsync();

            var message = JsonSerializer.Serialize(bookingConfirmed);
            var body = Encoding.UTF8.GetBytes(message);

            var props = new BasicProperties
            {
                Persistent = true,          // survive broker restart
                ContentType = "application/json",
                MessageId = bookingConfirmed.BookingId.ToString(),
                Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds())
            };

            await channel.BasicPublishAsync(
                exchange: ExchangeName,
                routingKey: string.Empty,
                mandatory: false,
                basicProperties: props,
                body: body);

            _logger.LogInformation(
                "Published BookingConfirmed event. BookingId={BookingId}, Seats={SeatCount}, Title={Title}",
                bookingConfirmed.BookingId, bookingConfirmed.SeatCount, bookingConfirmed.Title);
        }

        public async ValueTask DisposeAsync()
        {
            try
            {
                if (_channel is not null) await _channel.CloseAsync();
                if (_connection is not null) await _connection.CloseAsync();
            }
            catch { /* best-effort */ }
            finally
            {
                _channel?.Dispose();
                _connection?.Dispose();
                _initLock.Dispose();
            }
        }
    }
}