using CDM.BookVideo.Application.Interfaces;
using MediatR;

namespace CDM.BookVideo.Application.Commands {
  public class DeleteOrderCommandHandler : IRequestHandler<DeleteOrderCommand, bool> {
    private readonly IOrderRepository _repo;

    public DeleteOrderCommandHandler(IOrderRepository repo) {
      _repo = repo;
    }

    public async Task<bool> Handle(DeleteOrderCommand request, CancellationToken cancellationToken) {
      var order = await _repo.GetByIdAsync(request.OrderId);
      if (order == null) {
        throw new Exception(); // todo: introduce business exception
      }

      return await _repo.DeleteAsync(order);
    }
  }
}
