using Application.DTOs.Product;
using Application.Mappings;
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
		return product?.ToDto();
	}

	public async Task<IEnumerable<ProductDto>> GetAllAsync()
	{
		var products = await _productRepository.GetAllAsync();
		return products.Select(p => p.ToDto());
	}

	public async Task<PagedResult<ProductDto>> GetPagedAsync(int pageNumber, int pageSize)
	{
		var result = await _productRepository.GetPagedAsync(pageNumber, pageSize);
		var dtos = result.Items.Select(p => p.ToDto());
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
}
