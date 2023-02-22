using System.Text;
using System.Text.Json;
using CDM.BookVideo.Application.Events;
using CDM.BookVideo.Application.Events.EventHandling;
using CDM.BookVideo.EventBus;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace CDM.BookVideo.Application.Background {
  public class ShippingService : BackgroundService {
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IEventBusConnection _connection;
    private readonly ILogger<ShippingService> _logger;
    private IModel _consumerChannel;
    const string BROKER_NAME = "shipping_event_bus";
    private string _queueName = "Shipping";

    public ShippingService(IServiceScopeFactory scopeFactory, IEventBusConnection connection, ILogger<ShippingService> logger) {
      _scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
      _connection = connection ?? throw new ArgumentNullException(nameof(connection));
      _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken) {
      stoppingToken.ThrowIfCancellationRequested();
      if (!_connection.IsConnected) _connection.TryConnect();

      CreateConsumer();
      StartBasicConsume();
      return Task.CompletedTask;
    }

    public override void Dispose() {
      Console.WriteLine("--> Message Bus is disposed");
      if (_consumerChannel.IsOpen) {
        _consumerChannel.Close();
      }
    }

    private IModel? CreateConsumer() {
      if (!_connection.IsConnected) _connection.TryConnect();

      _consumerChannel = _connection.CreateModel();
      _consumerChannel.ExchangeDeclare(exchange: BROKER_NAME, type: ExchangeType.Direct);
      _consumerChannel.QueueDeclare(queue: _queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);

      _consumerChannel.CallbackException += (sender, ea) => {
        _consumerChannel.Dispose();
        _consumerChannel = CreateConsumer();
      };

      return _consumerChannel;
    }

    private void StartBasicConsume() {
      if (_consumerChannel != null) {
        var consumer = new AsyncEventingBasicConsumer(_consumerChannel);
        consumer.Received += ProcessMessage;

        _consumerChannel.BasicConsume(_queueName, false, consumer);
      }
      else {
        _logger.LogError("Consumer channel is not initialized.");
      }
    }

    private async Task ProcessMessage(object sender, BasicDeliverEventArgs @event) {
      var msg = Encoding.UTF8.GetString(@event.Body.Span);

      try {
        using var scope = _scopeFactory.CreateScope();
        var handler = scope.ServiceProvider.GetService<OrderPurchasedEventHandler>();
        var intEvent = JsonSerializer.Deserialize<OrderPurchasedEvent>(msg, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
        await handler.Handle(intEvent);
      }
      catch (Exception ex) {
        _logger.LogError("Error to process event witn message {Message}", msg);

        // Even on exception we take the message off the queue.
        // in a REAL WORLD app this should be handled with a Dead Letter Exchange (DLX). 
        // For more information see: https://www.rabbitmq.com/dlx.html
        _consumerChannel.BasicAck(@event.DeliveryTag, multiple: false);
      }
    }
  }
}
