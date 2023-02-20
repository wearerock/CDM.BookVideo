using CDM.BookVideo.Domain.Entities;
using CDM.BookVideo.EventBus.Events;

namespace CDM.BookVideo.Application.Events {
  public class OrderPurchasedEvent : IntegrationEvent {
    public int OrderId { get; set; }
    public decimal Total { get; set; }
    public int CustomerId { get; set; }
    public List<Product> Products { get; set; }

    public OrderPurchasedEvent(int orderId, decimal total, int customerId, List<Product> products) {
      OrderId = orderId;
      Total = total;
      CustomerId = customerId;
      Products = products;
    }
  }
}
