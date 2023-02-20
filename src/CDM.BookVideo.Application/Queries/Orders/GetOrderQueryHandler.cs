using CDM.BookVideo.Application.Interfaces;
using MediatR;

namespace CDM.BookVideo.Application.Queries.Orders {
  public class GetOrderQueryHandler : IRequestHandler<GetOrderQuery, GetOrderQueryResult> {
    private readonly IOrderRepository _repo;

    public GetOrderQueryHandler(IOrderRepository repo) {
      _repo = repo;
    }

    public async Task<GetOrderQueryResult> Handle(GetOrderQuery query, CancellationToken cancellationToken) {
      var order = await _repo.GetByIdAsync(query.OrderId);
      return new GetOrderQueryResult(order.OrderId, order.CustomerId, order.Total, order.Products);
    }
  }
}
