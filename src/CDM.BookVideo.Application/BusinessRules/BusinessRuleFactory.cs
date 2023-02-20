using CDM.BookVideo.Application.Interfaces.BusinessRules;
using CDM.BookVideo.EventBus;

namespace CDM.BookVideo.Application.BusinessRules {
  public class BusinessRuleFactory : IBusinessRuleFactory {
    private readonly IEventBus _eventBus;

    public BusinessRuleFactory(IEventBus eventBus) {
      _eventBus = eventBus;
    }

    public IBusinessRule GetBusinessRule() => new AggregateRule(new List<IBusinessRule>() { new MembershipBusinessRule(), new ShippmentBusinesRule(_eventBus) });
  }
}
