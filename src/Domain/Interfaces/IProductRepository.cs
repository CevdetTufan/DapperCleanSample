using Domain.Common;
using Domain.Entities;

namespace Domain.Interfaces;

public interface IProductRepository
{
	Task<Product?> GetByIdAsync(int id);
	Task<IEnumerable<Product>> GetAllAsync();
	Task<PagedResult<Product>> GetPagedAsync(int pageNumber, int pageSize);
	Task<int> AddAsync(Product product);
	Task<bool> UpdateAsync(Product product);
	Task<bool> DeleteAsync(int id);
}
