using System.Net;
using System.Net.Http.Json;
using Api.IntegrationTests.Fixtures;
using Application.DTOs.Customer;
using Application.DTOs.Order;
using Application.DTOs.Product;
using FluentAssertions;

namespace Api.IntegrationTests.Endpoints;

public class OrderEndpointTests : IClassFixture<ApiWebApplicationFactory>
{
	private readonly HttpClient _client;

	public OrderEndpointTests(ApiWebApplicationFactory factory)
	{
		_client = factory.CreateClient();
	}

	[Fact]
	public async Task CreateOrder_WithValidData_ReturnsCreated()
	{
		// Arrange
		var customerId = await CreateCustomerAsync();
		var productId = await CreateProductAsync();

		var request = new CreateOrderRequest(customerId,
		[
			new CreateOrderItemRequest(productId, 2, 100m)
		]);

		// Act
		var response = await _client.PostAsJsonAsync("/api/orders", request);

		// Assert
		response.StatusCode.Should().Be(HttpStatusCode.Created);
	}

	[Fact]
	public async Task GetOrdersPaged_ReturnsOk()
	{
		// Act
		var response = await _client.GetAsync("/api/orders/paged?pageNumber=1&pageSize=10");

		// Assert
		response.StatusCode.Should().Be(HttpStatusCode.OK);
	}

	[Fact]
	public async Task GetOrderById_WhenNotExists_ReturnsNotFound()
	{
		// Act
		var response = await _client.GetAsync("/api/orders/99999");

		// Assert
		response.StatusCode.Should().Be(HttpStatusCode.NotFound);
	}

	[Fact]
	public async Task GetOrderById_WhenExists_ReturnsOk()
	{
		// Arrange
		var orderId = await CreateOrderAsync();

		// Act
		var response = await _client.GetAsync($"/api/orders/{orderId}");

		// Assert
		response.StatusCode.Should().Be(HttpStatusCode.OK);
	}

	[Fact]
	public async Task GetOrderWithItems_ReturnsOk()
	{
		// Arrange
		var orderId = await CreateOrderAsync();

		// Act
		var response = await _client.GetAsync($"/api/orders/{orderId}/details");

		// Assert
		response.StatusCode.Should().Be(HttpStatusCode.OK);
	}

	[Fact]
	public async Task GetOrdersByCustomer_ReturnsOk()
	{
		// Arrange
		var customerId = await CreateCustomerAsync();
		var productId = await CreateProductAsync();
		var request = new CreateOrderRequest(customerId,
		[
			new CreateOrderItemRequest(productId, 1, 50m)
		]);
		await _client.PostAsJsonAsync("/api/orders", request);

		// Act
		var response = await _client.GetAsync($"/api/orders/customer/{customerId}");

		// Assert
		response.StatusCode.Should().Be(HttpStatusCode.OK);
	}

	[Fact]
	public async Task MarkOrderAsPaid_ReturnsNoContent()
	{
		// Arrange
		var orderId = await CreateOrderAsync();

		// Act
		var response = await _client.PatchAsync($"/api/orders/{orderId}/pay", null);

		// Assert
		response.StatusCode.Should().Be(HttpStatusCode.NoContent);
	}

	[Fact]
	public async Task MarkOrderAsPaid_WhenNotExists_ReturnsNotFound()
	{
		// Act
		var response = await _client.PatchAsync("/api/orders/99999/pay", null);

		// Assert
		response.StatusCode.Should().Be(HttpStatusCode.NotFound);
	}

	[Fact]
	public async Task ShipOrder_AfterPaid_ReturnsNoContent()
	{
		// Arrange
		var orderId = await CreateOrderAsync();
		await _client.PatchAsync($"/api/orders/{orderId}/pay", null);

		// Act
		var response = await _client.PatchAsync($"/api/orders/{orderId}/ship", null);

		// Assert
		response.StatusCode.Should().Be(HttpStatusCode.NoContent);
	}

	[Fact]
	public async Task DeliverOrder_AfterShipped_ReturnsNoContent()
	{
		// Arrange
		var orderId = await CreateOrderAsync();
		await _client.PatchAsync($"/api/orders/{orderId}/pay", null);
		await _client.PatchAsync($"/api/orders/{orderId}/ship", null);

		// Act
		var response = await _client.PatchAsync($"/api/orders/{orderId}/deliver", null);

		// Assert
		response.StatusCode.Should().Be(HttpStatusCode.NoContent);
	}

	[Fact]
	public async Task CancelOrder_WhenPending_ReturnsNoContent()
	{
		// Arrange
		var orderId = await CreateOrderAsync();

		// Act
		var response = await _client.PatchAsync($"/api/orders/{orderId}/cancel", null);

		// Assert
		response.StatusCode.Should().Be(HttpStatusCode.NoContent);
	}

	[Fact]
	public async Task OrderWorkflow_FullLifecycle_Succeeds()
	{
		// Arrange
		var orderId = await CreateOrderAsync();

		// Act & Assert - Pay
		var payResponse = await _client.PatchAsync($"/api/orders/{orderId}/pay", null);
		payResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

		// Act & Assert - Ship
		var shipResponse = await _client.PatchAsync($"/api/orders/{orderId}/ship", null);
		shipResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

		// Act & Assert - Deliver
		var deliverResponse = await _client.PatchAsync($"/api/orders/{orderId}/deliver", null);
		deliverResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
	}

	[Fact]
	public async Task DeleteOrder_WhenExists_ReturnsNoContent()
	{
		// Arrange
		var orderId = await CreateOrderAsync();

		// Act
		var response = await _client.DeleteAsync($"/api/orders/{orderId}");

		// Assert
		response.StatusCode.Should().Be(HttpStatusCode.NoContent);
	}

	[Fact]
	public async Task DeleteOrder_WhenNotExists_ReturnsNotFound()
	{
		// Act
		var response = await _client.DeleteAsync("/api/orders/99999");

		// Assert
		response.StatusCode.Should().Be(HttpStatusCode.NotFound);
	}

	#region Helper Methods

	private async Task<int> CreateCustomerAsync()
	{
		var request = new CreateCustomerRequest(
			$"Customer_{Guid.NewGuid()}",
			$"{Guid.NewGuid()}@test.com");
		var response = await _client.PostAsJsonAsync("/api/customers", request);
		var result = await response.Content.ReadFromJsonAsync<CreatedResponse>();
		return result!.Id;
	}

	private async Task<int> CreateProductAsync()
	{
		var request = new CreateProductRequest($"Product_{Guid.NewGuid()}", 100m);
		var response = await _client.PostAsJsonAsync("/api/products", request);
		var result = await response.Content.ReadFromJsonAsync<CreatedResponse>();
		return result!.Id;
	}

	private async Task<int> CreateOrderAsync()
	{
		var customerId = await CreateCustomerAsync();
		var productId = await CreateProductAsync();

		var request = new CreateOrderRequest(customerId,
		[
			new CreateOrderItemRequest(productId, 1, 100m)
		]);

		var response = await _client.PostAsJsonAsync("/api/orders", request);
		var result = await response.Content.ReadFromJsonAsync<CreatedResponse>();
		return result!.Id;
	}

	private record CreatedResponse(int Id);

	#endregion
}
