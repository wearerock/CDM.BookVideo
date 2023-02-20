using CDM.BookVideo.Application.Interfaces.BusinessRules;
using CDM.BookVideo.Domain.Entities;

namespace CDM.BookVideo.Application.BusinessRules {
  public class AggregateRule : IBusinessRule {
    private readonly IEnumerable<IBusinessRule> _rules;

    public AggregateRule(IEnumerable<IBusinessRule> rules) {
      _rules = rules;
    }

    public void Apply(Order order) => _rules.ToList().ForEach(x => x.Apply(order));
  }
}
