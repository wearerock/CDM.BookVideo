using MediatR;

namespace CDM.BookVideo.Application.Commands {
  public class CreateOrderCommand : IRequest<CreateOrderCommandResult> {

    public int CustomerId { get; }
    public decimal Total { get; }
    public List<string> Items { get; set; }

    public CreateOrderCommand(int customerId, decimal total, List<string> items) {
      CustomerId = customerId;
      Total = total;
      Items = items;
    }
  }
}
