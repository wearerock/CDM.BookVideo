using System.ComponentModel.DataAnnotations;

namespace CDM.BookVideo.Domain.Entities {
  public class Order {
    [Key]
    public int OrderId { get; set; }
    [Required]
    public decimal Total { get; set; }
    [Required]
    public int CustomerId { get; set; }
    public List<Product> Products { get; set; }
  }
}
