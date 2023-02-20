namespace CDM.BookVideo.EventBus.Events {
  public interface IIntegrationEventHandler<in T> where T : IntegrationEvent {
    Task Handle(T @event);
  }
}
