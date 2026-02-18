using Application.DTOs.Order;
using Application.Services;

namespace Api.Endpoints;

public class OrderEndpoints : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		var group = app.MapGroup("/api/orders")
			.WithTags("Orders");

		group.MapGet("/paged", GetPagedAsync)
			.WithName("GetOrdersPaged");

		group.MapGet("/{id:int}", GetByIdAsync)
			.WithName("GetOrderById");

		group.MapGet("/{id:int}/details", GetByIdWithItemsAsync)
			.WithName("GetOrderWithItems");

		group.MapGet("/customer/{customerId:int}", GetByCustomerIdAsync)
			.WithName("GetOrdersByCustomer");

		group.MapPost("/", CreateAsync)
			.WithName("CreateOrder");

		group.MapPatch("/{id:int}/pay", MarkAsPaidAsync)
			.WithName("MarkOrderAsPaid");

		group.MapPatch("/{id:int}/ship", ShipAsync)
			.WithName("ShipOrder");

		group.MapPatch("/{id:int}/deliver", DeliverAsync)
			.WithName("DeliverOrder");

		group.MapPatch("/{id:int}/cancel", CancelAsync)
			.WithName("CancelOrder");

		group.MapDelete("/{id:int}", DeleteAsync)
			.WithName("DeleteOrder");
	}

	private static async Task<IResult> GetPagedAsync(
		IOrderService service,
		int pageNumber = 1,
		int pageSize = 10)
	{
		var result = await service.GetPagedAsync(pageNumber, pageSize);
		return Results.Ok(result);
	}

	private static async Task<IResult> GetByIdAsync(int id, IOrderService service)
	{
		var order = await service.GetByIdAsync(id);
		return order is not null ? Results.Ok(order) : Results.NotFound();
	}

	private static async Task<IResult> GetByIdWithItemsAsync(int id, IOrderService service)
	{
		var order = await service.GetByIdWithItemsAsync(id);
		return order is not null ? Results.Ok(order) : Results.NotFound();
	}

	private static async Task<IResult> GetByCustomerIdAsync(int customerId, IOrderService service)
	{
		var orders = await service.GetByCustomerIdAsync(customerId);
		return Results.Ok(orders);
	}

	private static async Task<IResult> CreateAsync(CreateOrderRequest request, IOrderService service)
	{
		var id = await service.CreateAsync(request);
		return Results.Created($"/api/orders/{id}", new { Id = id });
	}

	private static async Task<IResult> MarkAsPaidAsync(int id, IOrderService service)
	{
		var success = await service.MarkAsPaidAsync(id);
		return success ? Results.NoContent() : Results.NotFound();
	}

	private static async Task<IResult> ShipAsync(int id, IOrderService service)
	{
		var success = await service.ShipAsync(id);
		return success ? Results.NoContent() : Results.NotFound();
	}

	private static async Task<IResult> DeliverAsync(int id, IOrderService service)
	{
		var success = await service.DeliverAsync(id);
		return success ? Results.NoContent() : Results.NotFound();
	}

	private static async Task<IResult> CancelAsync(int id, IOrderService service)
	{
		var success = await service.CancelAsync(id);
		return success ? Results.NoContent() : Results.NotFound();
	}

	private static async Task<IResult> DeleteAsync(int id, IOrderService service)
	{
		var success = await service.DeleteAsync(id);
		return success ? Results.NoContent() : Results.NotFound();
	}
}
