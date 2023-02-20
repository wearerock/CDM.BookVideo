using System.ComponentModel.DataAnnotations;

namespace CDM.BookVideo.API.Requests {
  public class UpdateProductRequest {
    [Required]
    public int OrderId { get; set; }
    [Required]
    public int CutomerId { get; set; }
    [Required]
    public decimal Total { get; set; }
    public List<string> Products { get; set; }
  }
}
