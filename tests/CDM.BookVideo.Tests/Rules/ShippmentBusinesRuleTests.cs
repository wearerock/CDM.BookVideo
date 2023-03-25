using CDM.BookVideo.Application.BusinessRules;
using CDM.BookVideo.Application.Events;
using CDM.BookVideo.Domain.Entities;
using CDM.BookVideo.EventBus;
using Moq;

namespace CDM.BookVideo.Tests.Rules {
  public class ShippmentBusinesRuleTests {
    private readonly Mock<IEventBus> _eventBusMock;
    private readonly ShippmentBusinesRule _rule;
    public ShippmentBusinesRuleTests() {
      _eventBusMock= new Mock<IEventBus>();
      _rule = new ShippmentBusinesRule(_eventBusMock.Object);
    }

    [Fact]
    public async void Should_PublishShippingEvent() {
      // Arrenge
      var order = new Order() { OrderId = 1, CustomerId = 2, Total = 3, Products = new List<Product> { 
        new Product { ProductId = 1, Details = "Garry Potter" }, new Product { ProductId = 1, Details = "Book membership" }, new Product { ProductId = 1, Details = "Book membership" } } };

      // Act
      _rule.Apply(order); //Fix tests

      // Assert
      _eventBusMock.Verify(x => x.Publish(It.IsAny<OrderPurchasedEvent>()), Times.Once);
    }
  }
}
