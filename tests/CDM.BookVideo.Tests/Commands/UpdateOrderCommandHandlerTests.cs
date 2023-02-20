using CDM.BookVideo.Application.Commands;
using CDM.BookVideo.Application.Commands.Update;
using CDM.BookVideo.Application.Interfaces;
using CDM.BookVideo.Domain.Entities;
using Moq;

namespace CDM.BookVideo.Tests.Commands {
  public class UpdateOrderCommandHandlerTests {
    private readonly Mock<IOrderRepository> _repoMock;
    private readonly UpdateOrderCommandHandler _handler;
    public UpdateOrderCommandHandlerTests() {
      _repoMock = new Mock<IOrderRepository>();
      _handler = new UpdateOrderCommandHandler(_repoMock.Object);
    }

    [Fact]
    public async void Should_UpdateOrder() {
      var order = new Order() { OrderId = 2, CustomerId = 3, Total = 4, Products = new List<Product> { new Product { ProductId = 1, Details = "Garry Potter" } } };
      var command = new UpdateOrderCommand(2, 5, 4, new List<string> { "Book: Good Father" });
      _repoMock.Setup(x => x.UpdateAsync(It.IsAny<Order>()));
      _repoMock.Setup(x => x.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(order);

      var result = await _handler.Handle(command, new CancellationToken());

      Assert.Equal(5, result.CunsomerId);
      Assert.Single(result.Products);
      Assert.Equal("Book: Good Father", result.Products.First());
    }
  }
}
