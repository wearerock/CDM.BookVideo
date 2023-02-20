using CDM.BookVideo.EventBus.Events;

namespace CDM.BookVideo.Application.Events.EventHandling {
  public class OrderPurchasedEventHandler : IIntegrationEventHandler<OrderPurchasedEvent> {
    public Task Handle(OrderPurchasedEvent @event) {
      Console.WriteLine("Start delivering...");
      return Task.CompletedTask;
    }
  }
}
