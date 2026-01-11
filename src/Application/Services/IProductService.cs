using Application.DTOs.Product;
using Domain.Common;

namespace Application.Services;

public interface IProductService
{
	Task<ProductDto?> GetByIdAsync(int id);
	Task<IEnumerable<ProductDto>> GetAllAsync();
	Task<PagedResult<ProductDto>> GetPagedAsync(int pageNumber, int pageSize);
	Task<int> CreateAsync(CreateProductRequest request);
	Task<bool> UpdateAsync(int id, UpdateProductRequest request);
	Task<bool> DeleteAsync(int id);
}
