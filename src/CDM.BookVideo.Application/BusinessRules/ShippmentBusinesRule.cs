using CDM.BookVideo.Application.Events;
using CDM.BookVideo.Application.Interfaces.BusinessRules;
using CDM.BookVideo.Domain.Entities;
using CDM.BookVideo.EventBus;

namespace CDM.BookVideo.Application.BusinessRules {
  public class ShippmentBusinesRule : IBusinessRule {
    private readonly IEventBus _eventBus;

    public ShippmentBusinesRule(IEventBus eventBus) {
      _eventBus = eventBus;
    }

    public void Apply(Order order) {
      if (order.Products.Any()) {
        Console.WriteLine("Senting signal to Shippment...");
        _eventBus.Publish(new OrderPurchasedEvent(order.OrderId, order.Total, order.CustomerId, order.Products));
      }
    }
  }
}
