using CDM.BookVideo.Domain.Entities;

namespace CDM.BookVideo.Application.Interfaces.BusinessRules {
  public interface IBusinessRule {
    void Apply(Order order);
  }
}
