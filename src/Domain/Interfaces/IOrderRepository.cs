using Domain.Common;
using Domain.Entities;

namespace Domain.Interfaces;

public interface IOrderRepository
{
	Task<Order?> GetByIdAsync(int id);
	Task<Order?> GetByIdWithItemsAsync(int id);
	Task<IEnumerable<Order>> GetByCustomerIdAsync(int customerId);
	Task<IEnumerable<Order>> GetAllAsync();
	Task<PagedResult<Order>> GetPagedAsync(int pageNumber, int pageSize);
	Task<PagedResult<Order>> GetPagedByCustomerIdAsync(int customerId, int pageNumber, int pageSize);
	Task<int> AddAsync(Order order);
	Task<bool> UpdateAsync(Order order);
	Task<bool> DeleteAsync(int id);
}
