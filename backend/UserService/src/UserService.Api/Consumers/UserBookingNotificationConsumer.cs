using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using Shared.Contracts.Events;
using System.Text;
using System.Text.Json;

namespace UserService.Api.Consumers
{
    /// <summary>
    /// Background service that listens for messages from RabbitMQ.
    /// It automatically retries connecting if RabbitMQ is not ready when the service starts.
    /// </summary>
    public class UserBookingNotificationConsumer(IConfiguration configuration) : BackgroundService
    {
        #region Configuration
        private readonly IConfiguration _configuration = configuration;

        // Holds the connection to RabbitMQ
        private IConnection? _connection;

        // Holds the channel used to receive messages
        private IChannel? _channel;
        #endregion

        /// <summary>
        /// Main method that runs when the background service starts.
        /// It connects to RabbitMQ, sets up the queue, and starts listening for messages.
        /// </summary>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Read RabbitMQ settings from configuration (appsettings.json)
            var host = _configuration["RabbitMQHost"] ?? "localhost";
            var portString = _configuration["RabbitMQPort"] ?? "5672";
            var port = int.Parse(portString);

            // Create a connection factory with the host and port
            var factory = new ConnectionFactory
            {
                HostName = host,
                Port = port
            };

            // Keep trying to connect until RabbitMQ becomes available
            // This is useful when the app starts before RabbitMQ is ready
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    // Try to create a connection and a channel
                    _connection = await factory.CreateConnectionAsync(stoppingToken);
                    _channel = await _connection.CreateChannelAsync(cancellationToken: stoppingToken);

                    // Connection successful → exit the retry loop
                    break;
                }
                catch (BrokerUnreachableException)
                {
                    // RabbitMQ is not ready yet → wait 5 seconds and try again
                    Console.WriteLine("RabbitMQ is not reachable yet. Retrying in 5 seconds...");

                    await Task.Delay(5000, stoppingToken);
                }
            }

            // If we still don't have a connection, stop the service
            if (_channel == null || _connection == null) return;

            // Create a Fanout exchange named "trigger"
            // Fanout means every queue bound to it will receive a copy of the message
            await _channel.ExchangeDeclareAsync(
                exchange: "trigger",
                type: ExchangeType.Fanout,
                durable: true,  // Survive RabbitMQ restarts
                autoDelete: false,
                cancellationToken: stoppingToken
            );

            // Create a temporary queue (RabbitMQ will generate a unique name)
            var queue = await _channel.QueueDeclareAsync(
                  queue: "booking.analytics", // <--- Named queue 
                  durable: true,
                  exclusive: false,
                  autoDelete: false,
                  cancellationToken: stoppingToken
            );

            // Bind the queue to the "trigger" exchange
            // Empty routing key is required for Fanout exchanges
            await _channel.QueueBindAsync(
                queue: queue.QueueName,
                exchange: "trigger",
                routingKey: "",
                cancellationToken: stoppingToken
            );

            // Create a consumer that will handle incoming messages
            var consumer = new AsyncEventingBasicConsumer(_channel);

            // This method runs every time a new message arrives
            consumer.ReceivedAsync += async (model, ea) =>
            {
                try
                {
                    // Convert the message body from bytes to a string
                    var body = ea.Body.ToArray();
                    var json = Encoding.UTF8.GetString(body);

                    // Convert the JSON string into a BookingConfirmed object
                    var booking = JsonSerializer.Deserialize<BookingConfirmed>(json);

                    Console.WriteLine("=========================================");
                    Console.WriteLine("Booking Event Received in UserService!");
                    Console.WriteLine($"BookingId : {booking?.BookingId}");
                    Console.WriteLine($"Movie     : {booking?.Title}");
                    Console.WriteLine($"Show Time : {booking?.ShowTime}");
                    Console.WriteLine($"Seats     : {booking?.SeatCount}");
                    Console.WriteLine("=========================================");

                    // 3. Success aana aprom manual ack
                    await _channel.BasicAckAsync(ea.DeliveryTag, false, stoppingToken);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error processing message: {ex.Message}");
                    // 4. Error aana Nack pannunga
                    await _channel.BasicNackAsync(ea.DeliveryTag, false, requeue: true, stoppingToken);
                }
            };

            // Start listening for messages on the queue
            // autoAck: true means messages are automatically acknowledged
            await _channel.BasicConsumeAsync(
                queue: queue.QueueName,
                autoAck: false,
                consumer: consumer,
                cancellationToken: stoppingToken
            );

            // Keep the service running until it is stopped
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }

        /// <summary>
        /// Stops the background service and closes RabbitMQ connections safely.
        /// </summary>
        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            if (_channel != null)
                await _channel.CloseAsync(cancellationToken);

            if (_connection != null)
                await _connection.CloseAsync(cancellationToken);

            await base.StopAsync(cancellationToken);
        }
    }
}