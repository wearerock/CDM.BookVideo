using MediatR;

namespace CDM.BookVideo.Application.Commands {
  public class DeleteOrderCommand : IRequest<bool>{
    public int OrderId { get; set; }
    public DeleteOrderCommand(int orderId) {
      OrderId = orderId;
    }
  }
}
