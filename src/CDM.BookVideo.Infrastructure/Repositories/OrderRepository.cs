using CDM.BookVideo.Application.Interfaces;
using CDM.BookVideo.Domain;
using CDM.BookVideo.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CDM.BookVideo.Infrastructure.Repositories {
  public class OrderRepository : IOrderRepository {
    private readonly BookVideoContext _context;

    public OrderRepository(BookVideoContext context) {
      _context = context ?? throw new ArgumentNullException(nameof(context));
    }
    public async Task<Order> CreateAsync(Order order) {
      _context.Orders.Add(order);
      await _context.SaveChangesAsync();
      return order;
    }

    public async Task<bool> DeleteAsync(Order order) {
      _context.Orders.Remove(order);
      return await SaveChangesAsync();
    }

    public async Task<IEnumerable<Order>> GetAllAsync() => _context.Orders.Include(x => x.Products).AsNoTracking();

    public async Task<Order> GetByIdAsync(int orderId) => _context.Orders.FirstOrDefault(x => x.OrderId == orderId);

    public async Task UpdateAsync(Order order) {
      _context.Update(order);
      await _context.SaveChangesAsync();
    }
    
    public async Task<bool> SaveChangesAsync() => await (_context.SaveChangesAsync()) >= 0;
  }
}
