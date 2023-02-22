using CDM.BookVideo.EventBus.Events;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace CDM.BookVideo.EventBus {
  public class EventBus : IEventBus, IDisposable {
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IEventBusConnection _connection;
    private readonly ILogger<EventBus> _logger;
    private readonly string _queueName;
    private readonly int _retryCount;

    private readonly List<Type> _events = new List<Type>();
    private readonly Dictionary<string, List<Type>> _handlers = new Dictionary<string, List<Type>>();
    private IModel _consumerChannel;

    const string BROKER_NAME = "shipping_event_bus";

    public EventBus(IServiceScopeFactory scopeFactory, IEventBusConnection connection, ILogger<EventBus> logger, string queueName, int retryCount = 3) {
      _scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
      _connection = connection ?? throw new ArgumentNullException(nameof(connection));
      _logger = logger ?? throw new ArgumentNullException(nameof(logger));
      _queueName = queueName;
      _retryCount = retryCount;

      _consumerChannel = CreateConsumer();
    }

    public void Publish(IntegrationEvent @event) {
      if (!_connection.IsConnected) _connection.TryConnect();

      var eventName = @event.GetType().Name;

      var policy = Policy.Handle<SocketException>().Or<BrokerUnreachableException>().WaitAndRetry(_retryCount, a => TimeSpan.FromSeconds(2), (e, t) => {
        _logger.LogError("Failed to publish event of {EventName}", eventName);
      });

      using var model = _connection.CreateModel();
      model.ExchangeDeclare(exchange: BROKER_NAME, type: ExchangeType.Direct);

      var body = JsonSerializer.SerializeToUtf8Bytes(@event, @event.GetType(), new JsonSerializerOptions() { WriteIndented = true });

      policy.Execute(() => {
        var props = model.CreateBasicProperties();
        props.DeliveryMode = 2;

        model.BasicPublish(BROKER_NAME, eventName, true, props, body);
      });
    }

    public void Subscribe<T, TH>() where T : IntegrationEvent where TH : IIntegrationEventHandler<T> {
      if (!_connection.IsConnected) _connection.TryConnect();

      var eventName = typeof(T).Name;
      _consumerChannel.QueueBind(_queueName, BROKER_NAME, eventName);

      if (!_events.Contains(typeof(T))) {
        _events.Add(typeof(T));
      }

      if (!_handlers.ContainsKey(eventName)) {
        _handlers.Add(eventName, new List<Type>());
      }

      if (!_handlers[eventName].Any(t => t == typeof(TH))) {
        _handlers[eventName].Add(typeof(TH));
      }

      StartBasicConsume();
    }

    public void UnSubscribe<T, TH>() where T : IntegrationEvent where TH : IIntegrationEventHandler<T> {
      var eventName = typeof(T).Name;
      var handler = _handlers.TryGetValue(eventName, out List<Type> h) ? h.SingleOrDefault(x => x == typeof(TH)) : null;
      if (handler != null) {
        _handlers[eventName].Remove(handler);
      }

      if (!_handlers[eventName].Any()) {
        _handlers.Remove(eventName);
        var eventType = _events.SingleOrDefault(x => x.Name == eventName);
        if (eventType != null) {
          _events.Remove(eventType);
        }
      }
    }

    public void Dispose() {
      _consumerChannel?.Dispose();
    }

    private IModel? CreateConsumer() {
      if (!_connection.IsConnected) _connection.TryConnect();

      var model = _connection.CreateModel();
      model.ExchangeDeclare(exchange: BROKER_NAME, type: ExchangeType.Direct);
      model.QueueDeclare(queue: _queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);

      model.CallbackException += (sender, ea) => {
        _consumerChannel.Dispose();
        _consumerChannel = CreateConsumer();
        StartBasicConsume();
      };

      return model;
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
        if (_handlers.TryGetValue(@event.RoutingKey, out var handlers)) {
          using var scope = _scopeFactory.CreateScope();
          Parallel.ForEach(handlers, async (handlerType) => {
            var handler = scope.ServiceProvider.GetService(handlerType);
            var eventType = _events.SingleOrDefault(x => x.Name == @event.RoutingKey);
            if (handler != null && eventType != null) {
              var intEvent = JsonSerializer.Deserialize(msg, eventType, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
              var instance = typeof(IIntegrationEventHandler<>).MakeGenericType(eventType);
              await (Task)instance.GetMethod("Handle").Invoke(handler, new object[] { intEvent });
            }
          });
        }
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
