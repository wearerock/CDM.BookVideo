using CDM.BookVideo.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CDM.BookVideo.Domain {
  public class BookVideoContext : DbContext {
    public DbSet<Order> Orders { get; set; }
    public BookVideoContext(DbContextOptions options) : base(options) {

    }
  }
}
