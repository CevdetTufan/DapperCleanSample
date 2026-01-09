using Domain.Common;
using Domain.Entities;
using Domain.Interfaces;

namespace Infrastructure.Repositories;

internal class CustomerRepository : ICustomerRepository
{
	public Task<int> AddAsync(Customer customer)
	{
		throw new NotImplementedException();
	}

	public Task<bool> DeleteAsync(int id)
	{
		throw new NotImplementedException();
	}

	public Task<IEnumerable<Customer>> GetAllAsync()
	{
		throw new NotImplementedException();
	}

	public Task<Customer?> GetByEmailAsync(string email)
	{
		throw new NotImplementedException();
	}

	public Task<Customer?> GetByIdAsync(int id)
	{
		throw new NotImplementedException();
	}

	public Task<PagedResult<Customer>> GetPagedAsync(int pageNumber, int pageSize)
	{
		throw new NotImplementedException();
	}

	public Task<bool> UpdateAsync(Customer customer)
	{
		throw new NotImplementedException();
	}
}
