using Application.DTOs.Order;
using Domain.Common;

namespace Application.Services;

public interface IOrderService
{
	Task<OrderDto?> GetByIdAsync(int id);
	Task<OrderDto?> GetByIdWithItemsAsync(int id);
	Task<IEnumerable<OrderDto>> GetByCustomerIdAsync(int customerId);
	Task<PagedResult<OrderDto>> GetPagedAsync(int pageNumber, int pageSize);
	Task<int> CreateAsync(CreateOrderRequest request);
	Task<bool> MarkAsPaidAsync(int id);
	Task<bool> ShipAsync(int id);
	Task<bool> DeliverAsync(int id);
	Task<bool> CancelAsync(int id);
	Task<bool> DeleteAsync(int id);
}
