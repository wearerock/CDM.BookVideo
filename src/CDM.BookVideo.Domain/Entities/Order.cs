using System.ComponentModel.DataAnnotations;

namespace CDM.BookVideo.Domain.Entities {
  public class Order {
    [Key]
    public int OrderId { get; set; }
    [Required]
    public decimal Total { get; set; }
    [Required]
    public int CunsomerId { get; set; }
    public List<Product> Products { get; set; }
  }
}
