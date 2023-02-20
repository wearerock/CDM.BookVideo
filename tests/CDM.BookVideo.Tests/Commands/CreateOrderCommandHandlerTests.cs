using CDM.BookVideo.Application.Commands;
using CDM.BookVideo.Application.Interfaces;
using CDM.BookVideo.Application.Interfaces.BusinessRules;
using CDM.BookVideo.Domain.Entities;
using Moq;

namespace CDM.BookVideo.Tests.Commands {
  public class CreateOrderCommandHandlerTests {
    private readonly Mock<IOrderRepository> _repoMock;
    private readonly Mock<IBusinessRuleFactory> _ruleFactoryMock;
    private readonly CreateOrderCommandHandler _handler;


    public CreateOrderCommandHandlerTests() {
      _repoMock = new Mock<IOrderRepository>();
      _ruleFactoryMock = new Mock<IBusinessRuleFactory>();
      _handler = new CreateOrderCommandHandler(_repoMock.Object, _ruleFactoryMock.Object);
    }

    [Fact]
    public async void Should_CreateOrder() {
      // Arrange
      var createdOrder = new Order() { OrderId = 1, CustomerId = 1, Total = 2, Products = new List<Product> { new Product { ProductId = 1, Details = "Book: Good Father" } } };
      var command = new CreateOrderCommand(1, 2, new List<string> { "Book: Good Father" });
      var ruleMock = new Mock<IBusinessRule>();
      _ruleFactoryMock.Setup(x => x.GetBusinessRule()).Returns(ruleMock.Object);
      _repoMock.Setup(x => x.CreateAsync(It.IsAny<Order>())).ReturnsAsync(createdOrder);

      // Act
      var result = await _handler.Handle(command, new CancellationToken());

      // Assert
      Assert.Single(result.Products);
      Assert.Equal("Book: Good Father", result.Products.First());
      _ruleFactoryMock.Verify(x => x.GetBusinessRule(), Times.Once());
      ruleMock.Verify(x => x.Apply(It.IsAny<Order>()), Times.Once);
    }
  }
}
