using Application.DTOs.Product;
using Domain.Common;
using Domain.Entities;
using Domain.Interfaces;

namespace Application.Services;

public class ProductService : IProductService
{
	private readonly IProductRepository _productRepository;

	public ProductService(IProductRepository productRepository)
	{
		_productRepository = productRepository;
	}

	public async Task<ProductDto?> GetByIdAsync(int id)
	{
		var product = await _productRepository.GetByIdAsync(id);
		return product is null ? null : MapToDto(product);
	}

	public async Task<IEnumerable<ProductDto>> GetAllAsync()
	{
		var products = await _productRepository.GetAllAsync();
		return products.Select(MapToDto);
	}

	public async Task<PagedResult<ProductDto>> GetPagedAsync(int pageNumber, int pageSize)
	{
		var result = await _productRepository.GetPagedAsync(pageNumber, pageSize);
		var dtos = result.Items.Select(MapToDto);
		return new PagedResult<ProductDto>(dtos, pageNumber, pageSize, result.TotalCount);
	}

	public async Task<int> CreateAsync(CreateProductRequest request)
	{
		var product = new Product(request.Name, request.Price);
		return await _productRepository.AddAsync(product);
	}

	public async Task<bool> UpdateAsync(int id, UpdateProductRequest request)
	{
		var product = await _productRepository.GetByIdAsync(id);
		if (product is null)
			return false;

		product.UpdateName(request.Name);
		product.UpdatePrice(request.Price);
		return await _productRepository.UpdateAsync(product);
	}

	public async Task<bool> DeleteAsync(int id)
	{
		return await _productRepository.DeleteAsync(id);
	}

	private static ProductDto MapToDto(Product product)
	{
		return new ProductDto(
			product.Id,
			product.Name,
			product.Price,
			product.CreatedAt
		);
	}
}
