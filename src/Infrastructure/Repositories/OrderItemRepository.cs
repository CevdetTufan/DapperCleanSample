using Domain.Entities;
using Domain.Interfaces;

namespace Infrastructure.Repositories;

internal class OrderItemRepository : IOrderItemRepository
{
	public Task<int> AddAsync(OrderItem orderItem)
	{
		throw new NotImplementedException();
	}

	public Task<bool> DeleteAsync(int id)
	{
		throw new NotImplementedException();
	}

	public Task<bool> DeleteByOrderIdAsync(int orderId)
	{
		throw new NotImplementedException();
	}

	public Task<OrderItem?> GetByIdAsync(int id)
	{
		throw new NotImplementedException();
	}

	public Task<IEnumerable<OrderItem>> GetByOrderIdAsync(int orderId)
	{
		throw new NotImplementedException();
	}

	public Task<bool> UpdateAsync(OrderItem orderItem)
	{
		throw new NotImplementedException();
	}
}
