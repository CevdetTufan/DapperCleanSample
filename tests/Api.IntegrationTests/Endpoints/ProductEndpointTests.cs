using System.Net;
using System.Net.Http.Json;
using Api.IntegrationTests.Fixtures;
using Application.DTOs.Product;
using FluentAssertions;

namespace Api.IntegrationTests.Endpoints;

public class ProductEndpointTests : IClassFixture<ApiWebApplicationFactory>
{
	private readonly HttpClient _client;

	public ProductEndpointTests(ApiWebApplicationFactory factory)
	{
		_client = factory.CreateClient();
	}

	[Fact]
	public async Task CreateProduct_WithValidData_ReturnsCreated()
	{
		// Arrange
		var request = new CreateProductRequest($"Product_{Guid.NewGuid()}", 1500.00m);

		// Act
		var response = await _client.PostAsJsonAsync("/api/products", request);

		// Assert
		response.StatusCode.Should().Be(HttpStatusCode.Created);
	}

	[Fact]
	public async Task GetAllProducts_ReturnsOk()
	{
		// Act
		var response = await _client.GetAsync("/api/products");

		// Assert
		response.StatusCode.Should().Be(HttpStatusCode.OK);
	}

	[Fact]
	public async Task GetProductsPaged_ReturnsOk()
	{
		// Act
		var response = await _client.GetAsync("/api/products/paged?pageNumber=1&pageSize=10");

		// Assert
		response.StatusCode.Should().Be(HttpStatusCode.OK);
	}

	[Fact]
	public async Task GetProductById_WhenNotExists_ReturnsNotFound()
	{
		// Act
		var response = await _client.GetAsync("/api/products/99999");

		// Assert
		response.StatusCode.Should().Be(HttpStatusCode.NotFound);
	}

	[Fact]
	public async Task CreateAndGetProduct_ReturnsCorrectData()
	{
		// Arrange
		var name = $"Laptop_{Guid.NewGuid()}";
		var price = 25000.50m;
		var request = new CreateProductRequest(name, price);
		var createResponse = await _client.PostAsJsonAsync("/api/products", request);
		var created = await createResponse.Content.ReadFromJsonAsync<CreatedResponse>();

		// Act
		var response = await _client.GetAsync($"/api/products/{created!.Id}");
		var product = await response.Content.ReadFromJsonAsync<ProductDto>();

		// Assert
		response.StatusCode.Should().Be(HttpStatusCode.OK);
		product!.Name.Should().Be(name);
		product.Price.Should().Be(price);
	}

	[Fact]
	public async Task UpdateProduct_WithValidData_ReturnsNoContent()
	{
		// Arrange
		var createRequest = new CreateProductRequest($"Update_{Guid.NewGuid()}", 100m);
		var createResponse = await _client.PostAsJsonAsync("/api/products", createRequest);
		var created = await createResponse.Content.ReadFromJsonAsync<CreatedResponse>();

		var updateRequest = new UpdateProductRequest("Updated Product", 999.99m);

		// Act
		var response = await _client.PutAsJsonAsync($"/api/products/{created!.Id}", updateRequest);

		// Assert
		response.StatusCode.Should().Be(HttpStatusCode.NoContent);
	}

	[Fact]
	public async Task UpdateProduct_WhenNotExists_ReturnsNotFound()
	{
		// Arrange
		var updateRequest = new UpdateProductRequest("Non Existing", 100m);

		// Act
		var response = await _client.PutAsJsonAsync("/api/products/99999", updateRequest);

		// Assert
		response.StatusCode.Should().Be(HttpStatusCode.NotFound);
	}

	[Fact]
	public async Task DeleteProduct_WhenExists_ReturnsNoContent()
	{
		// Arrange
		var request = new CreateProductRequest($"Delete_{Guid.NewGuid()}", 50m);
		var createResponse = await _client.PostAsJsonAsync("/api/products", request);
		var created = await createResponse.Content.ReadFromJsonAsync<CreatedResponse>();

		// Act
		var response = await _client.DeleteAsync($"/api/products/{created!.Id}");

		// Assert
		response.StatusCode.Should().Be(HttpStatusCode.NoContent);
	}

	[Fact]
	public async Task DeleteProduct_WhenNotExists_ReturnsNotFound()
	{
		// Act
		var response = await _client.DeleteAsync("/api/products/99999");

		// Assert
		response.StatusCode.Should().Be(HttpStatusCode.NotFound);
	}

	private record CreatedResponse(int Id);
}
