using System.Net.Sockets;
using Microsoft.Extensions.Logging;
using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;

namespace CDM.BookVideo.EventBus {
  public class EventBusConnection : IEventBusConnection {
    public bool IsConnected => _connection is { IsOpen: true } && !_disposed;

    private readonly ILogger<EventBusConnection> _logger;
    private readonly IConnectionFactory _factory;
    private readonly int _retryCount;
    private IConnection _connection;
    private bool _disposed;
    private object _lock = new object();

    public EventBusConnection(ILogger<EventBusConnection> logger, IConnectionFactory factory, int retryCount = 3) {
      _logger = logger ?? throw new ArgumentNullException(nameof(logger));
      _factory = factory ?? throw new ArgumentNullException(nameof(factory));
      _retryCount = retryCount;
    }

    public IModel CreateModel() {
      if (_connection == null || !IsConnected) {
        throw new InvalidOperationException("EventBus is not connected");
      }

      return _connection.CreateModel();
    }

    public bool TryConnect() {
      lock (_lock) {
        var policy = Policy.Handle<SocketException>().Or<BrokerUnreachableException>().WaitAndRetry(_retryCount, a => TimeSpan.FromSeconds(2), (ex, t) => {
          _logger.LogError("Can not connect to EventBus");
        });

        policy.Execute(() => {
          _connection = _factory.CreateConnection();
        });

        if (IsConnected) {
          _connection.CallbackException += OnCallbackException;
          _connection.ConnectionShutdown += OnConnectionShutdown;
          _logger.LogInformation("Connection to EventBus is established.");
          return true;
        }
        else {
          _logger.LogError("Connection to EventBus can not be established.");
          return false;
        }
      }
    }

    private void OnConnectionShutdown(object? sender, ShutdownEventArgs e) => HandleEvent("Connection is destroyed. Reconnecting.....");
    private void OnCallbackException(object? sender, CallbackExceptionEventArgs e) => HandleEvent("Connection closed error. Reconnecting....");

    private void HandleEvent(string message) {
      if (_disposed) return;

      _logger.LogWarning(message);

      TryConnect();
    }

    public void Dispose() {
      if (_disposed) return;

      try {
        _connection.ConnectionShutdown -= OnConnectionShutdown;
        _connection.CallbackException -= OnCallbackException;
        _connection.Dispose();
      }
      catch (Exception ex) {
        _logger.LogError(ex.ToString());
      }
    }
  }
}
