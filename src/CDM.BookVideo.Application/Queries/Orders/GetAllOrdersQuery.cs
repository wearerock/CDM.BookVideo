using MediatR;

namespace CDM.BookVideo.Application.Queries.Orders {
  public class GetAllOrdersQuery : IRequest<List<GetOrderQueryResult>> {
  }
}
