using CDM.BookVideo.EventBus.Events;

namespace CDM.BookVideo.Application.Events.EventHandling {
  // ACTUALLY HAS TO BE HANDLED IN SEPARATE SERVICE OR MAYBE BACKGROUND SERVICE
  public class OrderPurchasedEventHandler : IIntegrationEventHandler<OrderPurchasedEvent> {
    public Task Handle(OrderPurchasedEvent @event) {
      Console.WriteLine("Start delivering...");
      return Task.CompletedTask;
    }
  }
}
