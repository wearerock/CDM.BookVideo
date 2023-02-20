using RabbitMQ.Client;

namespace CDM.BookVideo.EventBus {
  public interface IEventBusConnection : IDisposable {
    bool IsConnected { get; }
    bool TryConnect();
    IModel CreateModel();
  }
}
