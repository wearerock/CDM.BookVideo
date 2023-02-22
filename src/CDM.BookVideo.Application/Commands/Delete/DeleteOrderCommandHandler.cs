using CDM.BookVideo.Application.Interfaces;
using CDM.BookVideo.Domain.Exceptions;
using MediatR;

namespace CDM.BookVideo.Application.Commands {
  public class DeleteOrderCommandHandler : IRequestHandler<DeleteOrderCommand, bool> {
    private readonly IOrderRepository _repo;

    public DeleteOrderCommandHandler(IOrderRepository repo) {
      _repo = repo;
    }

    public async Task<bool> Handle(DeleteOrderCommand command, CancellationToken cancellationToken) {
      var order = await _repo.GetByIdAsync(command.OrderId);
      if (order == null) {
        throw new OrderNotFoundException($"Order with ID {command.OrderId} is not found");
      }

      return await _repo.DeleteAsync(order);
    }
  }
}
