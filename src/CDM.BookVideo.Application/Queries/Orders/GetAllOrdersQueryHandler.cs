using CDM.BookVideo.Application.Interfaces;
using MediatR;

namespace CDM.BookVideo.Application.Queries.Orders {
  public class GetAllOrdersQueryHandler : IRequestHandler<GetAllOrdersQuery, List<GetOrderQueryResult>> {
    private readonly IOrderRepository _repo;

    public GetAllOrdersQueryHandler(IOrderRepository repo) {
      _repo = repo;
    }

    public async Task<List<GetOrderQueryResult>> Handle(GetAllOrdersQuery request, CancellationToken cancellationToken) {
      var orders = await _repo.GetAllAsync();
      return new List<GetOrderQueryResult>(orders.Select(x => new GetOrderQueryResult(x.OrderId, x.CunsomerId, x.Total, x.Products))).ToList();
    }
  }
}
