using CDM.BookVideo.Application.Interfaces;
using CDM.BookVideo.Application.Interfaces.BusinessRules;
using CDM.BookVideo.Domain.Entities;
using MediatR;

namespace CDM.BookVideo.Application.Commands {
  public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, CreateOrderCommandResult> {
    private readonly IOrderRepository _repo;
    private readonly IBusinessRuleFactory _ruleFactory;

    public CreateOrderCommandHandler(IOrderRepository repo, IBusinessRuleFactory ruleFactory) {
      _repo = repo;
      _ruleFactory = ruleFactory;
    }

    public async Task<CreateOrderCommandResult> Handle(CreateOrderCommand command, CancellationToken cancellationToken) {
      var order = new Order() {
        CustomerId = command.CustomerId,
        Total = command.Total,
        Products = command.Items.Select(x => new Product() { Details = x }).ToList()
      };

      await _repo.CreateAsync(order);

      var rule = _ruleFactory.GetBusinessRule();
      rule.Apply(order);

      return new CreateOrderCommandResult(order.OrderId, order.CustomerId, order.Total, order.Products);
    }
  }
}
