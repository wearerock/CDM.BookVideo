using System.Text;
using CDM.BookVideo.EventBus;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace CDM.BookVideo.Application.Background {
  public class ShippingService : BackgroundService {
    private readonly IEventBusConnection _connection;
    private readonly ILogger<ShippingService> _logger;
    private IModel _consumerChannel;
    const string BROKER_NAME = "shipping_event_bus";
    private string _queueName = "Shipping";

    public ShippingService(IEventBusConnection eventBusConnection, ILogger<ShippingService> logger) {
      _connection = eventBusConnection;
      _logger = logger;
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
        consumer.Received += (ModuleHandle, ea) => {
          Console.WriteLine($"--> Event recieved!");

          var body = ea.Body;
          var notificationMessage = Encoding.UTF8.GetString(body.ToArray());

          ProcessMessage(ModuleHandle, ea);

          return Task.CompletedTask;
        };

        _consumerChannel.BasicConsume(_queueName, false, consumer);
      }
      else {
        _logger.LogError("Consumer channel is not initialized.");
      }
    }

    private async Task ProcessMessage(object sender, BasicDeliverEventArgs @event) {
      var msg = Encoding.UTF8.GetString(@event.Body.Span);

      try {
        //if (_handlers.TryGetValue(@event.RoutingKey, out var handlers)) {
        //  using var scope = _scopeFactory.CreateScope();
        //  Parallel.ForEach(handlers, async (handlerType) => {
        //    var handler = scope.ServiceProvider.GetService(handlerType);
        //    var eventType = _events.SingleOrDefault(x => x.Name == @event.RoutingKey);
        //    if (handler != null && eventType != null) {
        //      var intEvent = JsonSerializer.Deserialize(msg, eventType, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
        //      var instance = typeof(IIntegrationEventHandler<>).MakeGenericType(eventType);
        //      await (Task)instance.GetMethod("Handle").Invoke(handler, new object[] { intEvent });
        //    }
        //  });
        //}
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
