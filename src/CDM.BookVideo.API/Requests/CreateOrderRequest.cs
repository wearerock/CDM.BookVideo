using System.ComponentModel.DataAnnotations;

namespace CDM.BookVideo.API.Requests {
  public class CreateOrderRequest {
    [Required]
    public int CutomerId { get; set; }
    [Required]
    public decimal Total { get; set; }
    public List<string> Products { get; set; }

  }
}
