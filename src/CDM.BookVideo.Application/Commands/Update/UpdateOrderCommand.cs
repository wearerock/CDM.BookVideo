using MediatR;

namespace CDM.BookVideo.Application.Commands {
  public class UpdateOrderCommand : IRequest<UpdateOrderCommandResult> {
    public int OrderId { get; }
    public int CutomerId { get; }
    public decimal Total { get; }
    public List<string> Products { get; }

    public UpdateOrderCommand(int orderId, int cutomerId, decimal total, List<string> products) {
      OrderId = orderId;
      CutomerId = cutomerId;
      Total = total;
      Products = products;
    }
  }
}
