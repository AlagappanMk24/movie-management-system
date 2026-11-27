using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Shared.Contracts.Events;
using System.Text;
using System.Text.Json;
using TheatreMasterService.Application.Abstractions.Services;

namespace TheatreMasterService.Api.Workers
{
    /// <summary>
    /// Background worker service that subscribes to RabbitMQ trigger exchanges to consume booking confirmation events.
    /// </summary>
    public class BookingConfirmedConsumer(IServiceScopeFactory scopeFactory, IConfiguration configuration) : BackgroundService
    {
        #region Configuration
        private readonly IServiceScopeFactory _scopeFactory = scopeFactory;
        private IConnection? _connection;
        private IChannel? _channel;
        private readonly IConfiguration _configuration = configuration;
        #endregion

        /// <inheritdoc />
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                var factory = new ConnectionFactory
                {
                    HostName = _configuration["RabbitMQHost"],
                    Port = int.Parse(_configuration["RabbitMQPort"]!)
                };

                _connection = await factory.CreateConnectionAsync(stoppingToken);
                _channel = await _connection.CreateChannelAsync(cancellationToken: stoppingToken);

                await _channel.ExchangeDeclareAsync("trigger", ExchangeType.Fanout);
                var queue = await _channel.QueueDeclareAsync();

                await _channel.QueueBindAsync(queue.QueueName, "trigger", string.Empty);

                var consumer = new AsyncEventingBasicConsumer(_channel);
                consumer.ReceivedAsync += async (sender, ea) =>
                {
                    var json = Encoding.UTF8.GetString(ea.Body.ToArray());
                    var booking = JsonSerializer.Deserialize<BookingConfirmed>(json);

                    if (booking != null)
                        await ReduceSeatsAsync(booking);

                    await Task.CompletedTask;
                };

                await _channel.BasicConsumeAsync(queue.QueueName, true, consumer);
                await Task.Delay(Timeout.Infinite, stoppingToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine("RabbitMQ error: " + ex.Message);
                await Task.Delay(Timeout.Infinite, stoppingToken);
            }
        }

        /// <summary>
        /// Creates a scoped service provider to invoke the show service and update available seating inventory.
        /// </summary>
        /// <param name="booking">The deserialized <see cref="BookingConfirmed"/> event payload.</param>
        private async Task ReduceSeatsAsync(BookingConfirmed booking)
        {
            using var scope = _scopeFactory.CreateScope();

            var showService = scope.ServiceProvider.GetRequiredService<IShowService>();

            await showService.ReduceSeatsAsync(
                booking.ShowId,
                booking.SeatCount
            );

            Console.WriteLine(
                $"Seats reduced for Show {booking.ShowId} by {booking.SeatCount}");
        }
    }
}