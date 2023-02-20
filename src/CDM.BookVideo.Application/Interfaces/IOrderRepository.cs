using CDM.BookVideo.Domain.Entities;

namespace CDM.BookVideo.Application.Interfaces {
  public interface IOrderRepository {
    Task<Order> CreateAsync(Order order);
    Task<IEnumerable<Order>> GetAllAsync();
    Task<Order> GetByIdAsync(int orderId);
    Task UpdateAsync(Order order);
    Task<bool> DeleteAsync(Order order);
    Task<bool> SaveChangesAsync();
  }
}
