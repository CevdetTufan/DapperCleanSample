using Application.DTOs.Product;
using Domain.Entities;

namespace Application.Mappings;

public static class ProductMappingExtensions
{
	public static ProductDto ToDto(this Product product)
	{
		return new ProductDto(
			product.Id,
			product.Name,
			product.Price,
			product.CreatedAt
		);
	}
}
