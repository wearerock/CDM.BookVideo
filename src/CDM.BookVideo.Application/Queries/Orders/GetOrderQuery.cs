using MediatR;

namespace CDM.BookVideo.Application.Queries.Orders {
  public class GetOrderQuery : IRequest<GetOrderQueryResult> {
    public int OrderId { get; }
    public GetOrderQuery(int orderId) {
      OrderId = orderId;
    }
  }
}
