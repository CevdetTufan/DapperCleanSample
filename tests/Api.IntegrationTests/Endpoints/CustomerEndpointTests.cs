using System.Net;
using System.Net.Http.Json;
using Api.IntegrationTests.Fixtures;
using Application.DTOs.Customer;
using FluentAssertions;

namespace Api.IntegrationTests.Endpoints;

public class CustomerEndpointTests : IClassFixture<ApiWebApplicationFactory>
{
	private readonly HttpClient _client;

	public CustomerEndpointTests(ApiWebApplicationFactory factory)
	{
		_client = factory.CreateClient();
	}

	[Fact]
	public async Task CreateCustomer_WithValidData_ReturnsCreated()
	{
		// Arrange
		var request = new CreateCustomerRequest($"John_{Guid.NewGuid()}", $"{Guid.NewGuid()}@example.com");

		// Act
		var response = await _client.PostAsJsonAsync("/api/customers", request);

		// Assert
		response.StatusCode.Should().Be(HttpStatusCode.Created);
	}

	[Fact]
	public async Task GetAllCustomers_ReturnsOk()
	{
		// Act
		var response = await _client.GetAsync("/api/customers");

		// Assert
		response.StatusCode.Should().Be(HttpStatusCode.OK);
	}

	[Fact]
	public async Task GetCustomersPaged_ReturnsOk()
	{
		// Act
		var response = await _client.GetAsync("/api/customers/paged?pageNumber=1&pageSize=10");

		// Assert
		response.StatusCode.Should().Be(HttpStatusCode.OK);
	}

	[Fact]
	public async Task GetCustomerById_WhenNotExists_ReturnsNotFound()
	{
		// Act
		var response = await _client.GetAsync("/api/customers/99999");

		// Assert
		response.StatusCode.Should().Be(HttpStatusCode.NotFound);
	}

	[Fact]
	public async Task CreateAndGetCustomer_ReturnsCorrectData()
	{
		// Arrange
		var name = $"Jane_{Guid.NewGuid()}";
		var email = $"{Guid.NewGuid()}@example.com";
		var request = new CreateCustomerRequest(name, email);
		var createResponse = await _client.PostAsJsonAsync("/api/customers", request);
		var created = await createResponse.Content.ReadFromJsonAsync<CreatedResponse>();

		// Act
		var response = await _client.GetAsync($"/api/customers/{created!.Id}");
		var customer = await response.Content.ReadFromJsonAsync<CustomerDto>();

		// Assert
		response.StatusCode.Should().Be(HttpStatusCode.OK);
		customer!.Name.Should().Be(name);
		customer.Email.Should().Be(email);
	}

	[Fact]
	public async Task GetCustomerByEmail_ReturnsCorrectData()
	{
		// Arrange
		var email = $"{Guid.NewGuid()}@example.com";
		var request = new CreateCustomerRequest("Email Test", email);
		await _client.PostAsJsonAsync("/api/customers", request);

		// Act
		var response = await _client.GetAsync($"/api/customers/email/{email}");

		// Assert
		response.StatusCode.Should().Be(HttpStatusCode.OK);
	}

	[Fact]
	public async Task UpdateCustomer_WithValidData_ReturnsNoContent()
	{
		// Arrange
		var createRequest = new CreateCustomerRequest($"Update_{Guid.NewGuid()}", $"{Guid.NewGuid()}@example.com");
		var createResponse = await _client.PostAsJsonAsync("/api/customers", createRequest);
		var created = await createResponse.Content.ReadFromJsonAsync<CreatedResponse>();

		var updateRequest = new UpdateCustomerRequest("Updated Name", $"{Guid.NewGuid()}@updated.com");

		// Act
		var response = await _client.PutAsJsonAsync($"/api/customers/{created!.Id}", updateRequest);

		// Assert
		response.StatusCode.Should().Be(HttpStatusCode.NoContent);
	}

	[Fact]
	public async Task DeleteCustomer_WhenExists_ReturnsNoContent()
	{
		// Arrange
		var request = new CreateCustomerRequest($"Delete_{Guid.NewGuid()}", $"{Guid.NewGuid()}@example.com");
		var createResponse = await _client.PostAsJsonAsync("/api/customers", request);
		var created = await createResponse.Content.ReadFromJsonAsync<CreatedResponse>();

		// Act
		var response = await _client.DeleteAsync($"/api/customers/{created!.Id}");

		// Assert
		response.StatusCode.Should().Be(HttpStatusCode.NoContent);
	}

	[Fact]
	public async Task DeleteCustomer_WhenNotExists_ReturnsNotFound()
	{
		// Act
		var response = await _client.DeleteAsync("/api/customers/99999");

		// Assert
		response.StatusCode.Should().Be(HttpStatusCode.NotFound);
	}

	private record CreatedResponse(int Id);
}
