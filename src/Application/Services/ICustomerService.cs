using Application.DTOs.Customer;
using Domain.Common;

namespace Application.Services;

public interface ICustomerService
{
	Task<CustomerDto?> GetByIdAsync(int id);
	Task<CustomerDto?> GetByEmailAsync(string email);
	Task<IEnumerable<CustomerDto>> GetAllAsync();
	Task<PagedResult<CustomerDto>> GetPagedAsync(int pageNumber, int pageSize);
	Task<int> CreateAsync(CreateCustomerRequest request);
	Task<bool> UpdateAsync(int id, UpdateCustomerRequest request);
	Task<bool> DeleteAsync(int id);
}