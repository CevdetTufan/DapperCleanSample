using Domain.Entities;
using Domain.ValueObjects;
using FluentAssertions;
using Infrastructure.IntegrationTests.Fixtures;

namespace Infrastructure.IntegrationTests.Repositories;

public class CustomerRepositoryTests : IClassFixture<DatabaseFixture>, IDisposable
{
	private readonly DatabaseFixture _fixture;
	private readonly TestCustomerRepository _repository;

	public CustomerRepositoryTests(DatabaseFixture fixture)
	{
		_fixture = fixture;
		_repository = new TestCustomerRepository(_fixture.Connection);
		_fixture.ClearTables();
	}

	[Fact]
	public async Task AddAsync_ShouldInsertCustomerAndReturnId()
	{
		// Arrange
		var customer = new Customer("John Doe", new Email("john@example.com"));

		// Act
		var id = await _repository.AddAsync(customer);

		// Assert
		id.Should().BeGreaterThan(0);
	}

	[Fact]
	public async Task GetByIdAsync_WhenCustomerExists_ShouldReturnCustomer()
	{
		// Arrange
		var customer = new Customer("John Doe", new Email("john@example.com"));
		var id = await _repository.AddAsync(customer);

		// Act
		var result = await _repository.GetByIdAsync(id);

		// Assert
		result.Should().NotBeNull();
		result!.Name.Should().Be("John Doe");
	}

	[Fact]
	public async Task GetByEmailAsync_WhenCustomerExists_ShouldReturnCustomer()
	{
		// Arrange
		var customer = new Customer("John Doe", new Email("john@example.com"));
		await _repository.AddAsync(customer);

		// Act
		var result = await _repository.GetByEmailAsync("john@example.com");

		// Assert
		result.Should().NotBeNull();
		result!.Name.Should().Be("John Doe");
	}

	[Fact]
	public async Task GetByEmailAsync_WhenCustomerDoesNotExist_ShouldReturnNull()
	{
		// Act
		var result = await _repository.GetByEmailAsync("notfound@example.com");

		// Assert
		result.Should().BeNull();
	}

	[Fact]
	public async Task GetPagedAsync_ShouldReturnPagedResult()
	{
		// Arrange
		for (int i = 1; i <= 15; i++)
		{
			await _repository.AddAsync(new Customer($"Customer {i}", new Email($"customer{i}@example.com")));
		}

		// Act
		var result = await _repository.GetPagedAsync(1, 10);

		// Assert
		result.Items.Should().HaveCount(10);
		result.TotalCount.Should().Be(15);
		result.TotalPages.Should().Be(2);
	}

	[Fact]
	public async Task DeleteAsync_WhenCustomerExists_ShouldReturnTrue()
	{
		// Arrange
		var customer = new Customer("John Doe", new Email("john@example.com"));
		var id = await _repository.AddAsync(customer);

		// Act
		var result = await _repository.DeleteAsync(id);

		// Assert
		result.Should().BeTrue();
		var deleted = await _repository.GetByIdAsync(id);
		deleted.Should().BeNull();
	}

	public void Dispose()
	{
		_fixture.ClearTables();
	}
}