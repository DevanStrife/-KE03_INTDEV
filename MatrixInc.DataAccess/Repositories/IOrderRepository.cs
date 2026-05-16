using MatrixInc.DataAccess.Models;

namespace MatrixInc.DataAccess.Repositories;

public interface IOrderRepository
{
    Task<IEnumerable<Order>> GetAllAsync();
    Task<Order?> GetByIdAsync(int id);
    Task<IEnumerable<Order>> GetByCustomerIdAsync(int customerId);
    Task<int> CreateOrderAsync(Order order);
    Task UpdateAsync(Order order);
}
