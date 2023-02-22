using CDM.BookVideo.Application.Interfaces;
using CDM.BookVideo.Domain.Entities;
using CDM.BookVideo.Domain.Exceptions;
using MediatR;

namespace CDM.BookVideo.Application.Commands.Update {
  public class UpdateOrderCommandHandler : IRequestHandler<UpdateOrderCommand, UpdateOrderCommandResult> {
    private readonly IOrderRepository _repo;

    public UpdateOrderCommandHandler(IOrderRepository repo) {
      _repo = repo;
    }

    public async Task<UpdateOrderCommandResult> Handle(UpdateOrderCommand command, CancellationToken cancellationToken) {
      var order = await _repo.GetByIdAsync(command.OrderId);

      if (order == null) {
        throw new OrderNotFoundException($"Order with ID {command.OrderId} is not found");
      }

      order.Total = command.Total;
      order.CustomerId = command.CutomerId;
      order.Products = command.Products.Select(x => new Product() { Details = x }).ToList();
      await _repo.UpdateAsync(order);


      return new UpdateOrderCommandResult(order.OrderId, order.CustomerId, order.Total, order.Products);
    }
  }
}
