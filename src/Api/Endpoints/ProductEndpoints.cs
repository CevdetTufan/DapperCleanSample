using Application.DTOs.Product;
using Application.Services;

namespace Api.Endpoints;

public class ProductEndpoints : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		var group = app.MapGroup("/api/products")
			.WithTags("Products");

		group.MapGet("/", GetAllAsync)
			.WithName("GetAllProducts");

		group.MapGet("/paged", GetPagedAsync)
			.WithName("GetProductsPaged");

		group.MapGet("/{id:int}", GetByIdAsync)
			.WithName("GetProductById");

		group.MapPost("/", CreateAsync)
			.WithName("CreateProduct");

		group.MapPut("/{id:int}", UpdateAsync)
			.WithName("UpdateProduct");

		group.MapDelete("/{id:int}", DeleteAsync)
			.WithName("DeleteProduct");
	}

	private static async Task<IResult> GetAllAsync(IProductService service)
	{
		var products = await service.GetAllAsync();
		return Results.Ok(products);
	}

	private static async Task<IResult> GetPagedAsync(
		IProductService service,
		int pageNumber = 1,
		int pageSize = 10)
	{
		var result = await service.GetPagedAsync(pageNumber, pageSize);
		return Results.Ok(result);
	}

	private static async Task<IResult> GetByIdAsync(int id, IProductService service)
	{
		var product = await service.GetByIdAsync(id);
		return product is not null ? Results.Ok(product) : Results.NotFound();
	}

	private static async Task<IResult> CreateAsync(CreateProductRequest request, IProductService service)
	{
		var id = await service.CreateAsync(request);
		return Results.Created($"/api/products/{id}", new { Id = id });
	}

	private static async Task<IResult> UpdateAsync(int id, UpdateProductRequest request, IProductService service)
	{
		var success = await service.UpdateAsync(id, request);
		return success ? Results.NoContent() : Results.NotFound();
	}

	private static async Task<IResult> DeleteAsync(int id, IProductService service)
	{
		var success = await service.DeleteAsync(id);
		return success ? Results.NoContent() : Results.NotFound();
	}
}
