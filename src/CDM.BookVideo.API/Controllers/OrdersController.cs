using CDM.BookVideo.API.Requests;
using CDM.BookVideo.Application.Commands;
using CDM.BookVideo.Application.Queries.Orders;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace CDM.BookVideo.API.Controllers;

/// <summary>
/// Orders interaction
/// </summary>
[ApiController]
[Route("[controller]")]
public class OrdersController : ControllerBase {
  private readonly ISender _sender;

  /// <summary>
  /// Initializer
  /// </summary>
  /// <param name="sender">MediatR sender</param>
  public OrdersController(ISender sender) {
    _sender = sender;
  }
  
  /// <summary>
  /// Create new Order
  /// </summary>
  /// <param name="request"></param>
  /// <returns>Returns new order. <see cref="CreateOrderCommandResult"/></returns>
  [HttpPost()]
  [ProducesResponseType(typeof(CreateOrderCommandResult), StatusCodes.Status201Created)]
  [SwaggerResponse(StatusCodes.Status201Created, "New order created")]
  public async Task<IActionResult> Post(CreateOrderRequest request) {
    var query = new CreateOrderCommand(request.CutomerId, request.Total, request.Products);
    var result = await _sender.Send(query);
    return CreatedAtAction(nameof(Get), new { id = result.OrderId }, result);
  }

  /// <summary>
  /// Get all orders
  /// </summary>
  /// <returns>Returns list of <see cref="GetOrderQueryResult"/></returns>
  [HttpGet()]
  [ProducesResponseType(typeof(List<GetOrderQueryResult>), StatusCodes.Status200OK)]
  [SwaggerResponse(StatusCodes.Status200OK, "List of orders")]
  public async Task<IActionResult> Get() {
    var query = new GetAllOrdersQuery();
    var result = await _sender.Send(query);
    return Ok(result);
  }

  /// <summary>
  /// Retrieve order for given ID
  /// </summary>
  /// <param name="id">Order ID</param>
  /// <returns>Returns <see cref="GetOrderQueryResult"/></returns>
  [HttpGet("{id:int}")]
  [ProducesResponseType(typeof(GetOrderQueryResult), StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status404NotFound)]
  [SwaggerResponse(StatusCodes.Status200OK, "Order for given ID")]
  public async Task<IActionResult> Get(int id) {
    var query = new GetOrderQuery(id);
    var result = await _sender.Send(query);

    if (result == null) {
      return NotFound();
    }

    return Ok(result);
  }

  /// <summary>
  /// Update order
  /// </summary>
  /// <param name="request">Update order request</param>
  /// <returns>Returns <see cref="UpdateOrderCommandResult"/></returns>
  [HttpPut()]
  [ProducesResponseType(typeof(UpdateOrderCommandResult), StatusCodes.Status200OK)]
  [SwaggerResponse(StatusCodes.Status200OK, "Updated orders")]
  public async Task<IActionResult> Put(UpdateProductRequest request) {
    var query = new UpdateOrderCommand(request.OrderId, request.CutomerId, request.Total, request.Products);
    var result = await _sender.Send(query);
    return Ok(result);
  }

  /// <summary>
  /// Delete order by ID
  /// </summary>
  /// <param name="id">Order ID</param>
  /// <returns></returns>
  [HttpDelete("{id:int}")]
  [ProducesResponseType(StatusCodes.Status201Created)]
  [SwaggerResponse(StatusCodes.Status200OK, "Success result")]
  public async Task<IActionResult> Delete(int id) {
    var query = new DeleteOrderCommand(id);
    var result = await _sender.Send(query);
    return Ok(result);
  }

}
