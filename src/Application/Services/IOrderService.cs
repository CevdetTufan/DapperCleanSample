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
	Task MarkAsPaidAsync(int id);
	Task ShipAsync(int id);
	Task DeliverAsync(int id);
	Task CancelAsync(int id);
	Task DeleteAsync(int id);
}
