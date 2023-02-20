using CDM.BookVideo.Application.Interfaces.BusinessRules;
using CDM.BookVideo.Domain.Entities;

namespace CDM.BookVideo.Application.BusinessRules {
  public class MembershipBusinessRule : IBusinessRule {
    public void Apply(Order order) {
      var bookMember = order.Products.FirstOrDefault(x => x.Details.Contains("Book membership", StringComparison.OrdinalIgnoreCase));
      var videoMember = order.Products.FirstOrDefault(x => x.Details.Contains("Video membership", StringComparison.OrdinalIgnoreCase));
      if (bookMember != null)
        SignMembership(bookMember.Details);
      
      if (videoMember != null)
        SignMembership(videoMember.Details);
    }

    private void SignMembership(string memberType) {
      Console.WriteLine($"Applying membership for '{memberType}' type");
    }
  }
}
