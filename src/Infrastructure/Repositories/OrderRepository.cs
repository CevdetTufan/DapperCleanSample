using Domain.Common;
using Domain.Entities;
using Domain.Interfaces;

namespace Infrastructure.Repositories;

internal class OrderRepository : IOrderRepository
{
	public Task<int> AddAsync(Order order)
	{
		throw new NotImplementedException();
	}

	public Task<bool> DeleteAsync(int id)
	{
		throw new NotImplementedException();
	}

	public Task<IEnumerable<Order>> GetAllAsync()
	{
		throw new NotImplementedException();
	}

	public Task<IEnumerable<Order>> GetByCustomerIdAsync(int customerId)
	{
		throw new NotImplementedException();
	}

	public Task<Order?> GetByIdAsync(int id)
	{
		throw new NotImplementedException();
	}

	public Task<Order?> GetByIdWithItemsAsync(int id)
	{
		throw new NotImplementedException();
	}

	public Task<PagedResult<Order>> GetPagedAsync(int pageNumber, int pageSize)
	{
		throw new NotImplementedException();
	}

	public Task<PagedResult<Order>> GetPagedByCustomerIdAsync(int customerId, int pageNumber, int pageSize)
	{
		throw new NotImplementedException();
	}

	public Task<bool> UpdateAsync(Order order)
	{
		throw new NotImplementedException();
	}
}
