using Domain.Entities;

namespace Domain.Interfaces;

public interface IOrderItemRepository
{
	Task<OrderItem?> GetByIdAsync(int id);
	Task<IEnumerable<OrderItem>> GetByOrderIdAsync(int orderId);
	Task<int> AddAsync(OrderItem orderItem);
	Task<bool> UpdateAsync(OrderItem orderItem);
	Task<bool> DeleteAsync(int id);
	Task<bool> DeleteByOrderIdAsync(int orderId);
}
