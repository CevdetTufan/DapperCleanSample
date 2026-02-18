using Application.DTOs.Customer;
using Application.Services;

namespace Api.Endpoints;

public class CustomerEndpoints : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		var group = app.MapGroup("/api/customers")
			.WithTags("Customers");

		group.MapGet("/", GetAllAsync)
			.WithName("GetAllCustomers");

		group.MapGet("/paged", GetPagedAsync)
			.WithName("GetCustomersPaged");

		group.MapGet("/{id:int}", GetByIdAsync)
			.WithName("GetCustomerById");

		group.MapGet("/email/{email}", GetByEmailAsync)
			.WithName("GetCustomerByEmail");

		group.MapPost("/", CreateAsync)
			.WithName("CreateCustomer");

		group.MapPut("/{id:int}", UpdateAsync)
			.WithName("UpdateCustomer");

		group.MapDelete("/{id:int}", DeleteAsync)
			.WithName("DeleteCustomer");
	}

	private static async Task<IResult> GetAllAsync(ICustomerService service)
	{
		var customers = await service.GetAllAsync();
		return Results.Ok(customers);
	}

	private static async Task<IResult> GetPagedAsync(
		ICustomerService service,
		int pageNumber = 1,
		int pageSize = 10)
	{
		var result = await service.GetPagedAsync(pageNumber, pageSize);
		return Results.Ok(result);
	}

	private static async Task<IResult> GetByIdAsync(int id, ICustomerService service)
	{
		var customer = await service.GetByIdAsync(id);
		return customer is not null ? Results.Ok(customer) : Results.NotFound();
	}

	private static async Task<IResult> GetByEmailAsync(string email, ICustomerService service)
	{
		var customer = await service.GetByEmailAsync(email);
		return customer is not null ? Results.Ok(customer) : Results.NotFound();
	}

	private static async Task<IResult> CreateAsync(CreateCustomerRequest request, ICustomerService service)
	{
		var id = await service.CreateAsync(request);
		return Results.Created($"/api/customers/{id}", new { Id = id });
	}

	private static async Task<IResult> UpdateAsync(int id, UpdateCustomerRequest request, ICustomerService service)
	{
		var success = await service.UpdateAsync(id, request);
		return success ? Results.NoContent() : Results.NotFound();
	}

	private static async Task<IResult> DeleteAsync(int id, ICustomerService service)
	{
		var success = await service.DeleteAsync(id);
		return success ? Results.NoContent() : Results.NotFound();
	}
}
