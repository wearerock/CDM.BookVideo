using CDM.BookVideo.EventBus.Events;

namespace CDM.BookVideo.EventBus {
  public interface IEventBus {
    void Publish(IntegrationEvent @event);
    void Subscribe<T, TH>() where T : IntegrationEvent where TH : IIntegrationEventHandler<T>;
    void UnSubscribe<T, TH>() where T : IntegrationEvent where TH : IIntegrationEventHandler<T>;
  }
}
