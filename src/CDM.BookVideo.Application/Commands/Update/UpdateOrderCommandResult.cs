using CDM.BookVideo.Domain.Entities;

namespace CDM.BookVideo.Application.Commands {
  public class UpdateOrderCommandResult {
    public int OrderId { get; }
    public int CunsomerId { get; }
    public decimal Total { get; }
    public List<string> Products { get; }

    public UpdateOrderCommandResult(int orderId, int cunsomerId, decimal total, List<Product> products) {
      OrderId = orderId;
      CunsomerId = cunsomerId;
      Total = total;
      Products = products.Select(x => x.Details).ToList();
    }
  }
}
