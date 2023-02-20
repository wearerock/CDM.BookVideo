using System.ComponentModel.DataAnnotations;

namespace CDM.BookVideo.Domain.Entities {
  public class Product {
    [Key]
    public int ProductId { get; set; }
    [Required]
    public string Details { get; set; }
  }
}
