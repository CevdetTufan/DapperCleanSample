using Domain.Common;
using Domain.Entities;

namespace Domain.Interfaces;

public interface ICustomerRepository
{
	Task<Customer?> GetByIdAsync(int id);
	Task<Customer?> GetByEmailAsync(string email);
	Task<IEnumerable<Customer>> GetAllAsync();
	Task<PagedResult<Customer>> GetPagedAsync(int pageNumber, int pageSize);
	Task<int> AddAsync(Customer customer);
	Task<bool> UpdateAsync(Customer customer);
	Task<bool> DeleteAsync(int id);
}
