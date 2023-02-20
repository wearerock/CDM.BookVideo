using CDM.BookVideo.API.Controllers;
using CDM.BookVideo.Application.Queries.Orders;
using CDM.BookVideo.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace CDM.BookVideo.Tests.Controllers {
  public class OrdersControllerTests {
    private readonly OrdersController _controller;
    private readonly Mock<IMediator> _mediatorMock;
    public OrdersControllerTests() {
      _mediatorMock = new Mock<IMediator>();

      _controller = new OrdersController(_mediatorMock.Object);
    }

    [Fact]
    public async void Should_ReturnAllOrders_Success() {
      // Arrenge
      _mediatorMock.Setup(x => x.Send(It.IsAny<GetAllOrdersQuery>(), It.IsAny<CancellationToken>()))
                   .ReturnsAsync(new List<GetOrderQueryResult>() { new GetOrderQueryResult(1, 2, 3, new List<Product> { new Product { ProductId = 1, Details = "123" } } )} );
      // Act
      var result = await _controller.Get();

      // Assert
      Assert.IsType<OkObjectResult>(result);
      var okResult = result as OkObjectResult;
      Assert.IsType<List<GetOrderQueryResult>>(okResult?.Value);
      var orders = okResult.Value as List<GetOrderQueryResult>;
      Assert.Equal(1, orders?.Count);
    }

    [Fact]
    public async void Should_ReturnNoOrders_Success() {
      // Arrenge
      _mediatorMock.Setup(x => x.Send(It.IsAny<GetAllOrdersQuery>(), It.IsAny<CancellationToken>()))
                   .ReturnsAsync(new List<GetOrderQueryResult>());
      // Act
      var result = await _controller.Get();

      // Assert
      Assert.IsType<OkObjectResult>(result);
      var okResult = result as OkObjectResult;
      Assert.IsType<List<GetOrderQueryResult>>(okResult?.Value);
      var orders = okResult.Value as List<GetOrderQueryResult>;
      Assert.Equal(0, orders?.Count);
    }
  }
}
