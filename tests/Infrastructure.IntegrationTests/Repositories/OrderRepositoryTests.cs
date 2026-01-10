using Dapper;
using Domain.Entities;
using Domain.Enums;
using Domain.ValueObjects;
using FluentAssertions;
using Infrastructure.IntegrationTests.Fixtures;

namespace Infrastructure.IntegrationTests.Repositories;

public class OrderRepositoryTests : IClassFixture<DatabaseFixture>, IDisposable
{
	private readonly DatabaseFixture _fixture;
	private readonly TestOrderRepository _repository;
	private readonly TestCustomerRepository _customerRepository;

	public OrderRepositoryTests(DatabaseFixture fixture)
	{
		_fixture = fixture;
		_repository = new TestOrderRepository(_fixture.Connection);
		_customerRepository = new TestCustomerRepository(_fixture.Connection);
		_fixture.ClearTables();
	}

	private async Task<int> CreateTestCustomer()
	{
		var customer = new Customer("Test Customer", new Email("test@example.com"));
		return await _customerRepository.AddAsync(customer);
	}

	[Fact]
	public async Task AddAsync_ShouldInsertOrderAndReturnId()
	{
		// Arrange
		var customerId = await CreateTestCustomer();
		var order = new Order(customerId);

		// Act
		var id = await _repository.AddAsync(order);

		// Assert
		id.Should().BeGreaterThan(0);
	}

	[Fact]
	public async Task GetByIdAsync_WhenOrderExists_ShouldReturnOrder()
	{
		// Arrange
		var customerId = await CreateTestCustomer();
		var order = new Order(customerId);
		var id = await _repository.AddAsync(order);

		// Act
		var result = await _repository.GetByIdAsync(id);

		// Assert
		result.Should().NotBeNull();
		result!.CustomerId.Should().Be(customerId);
		result.Status.Should().Be(OrderStatus.Pending);
	}

	[Fact]
	public async Task GetByCustomerIdAsync_ShouldReturnCustomerOrders()
	{
		// Arrange
		var customerId = await CreateTestCustomer();
		await _repository.AddAsync(new Order(customerId));
		await _repository.AddAsync(new Order(customerId));
		await _repository.AddAsync(new Order(customerId));

		// Act
		var result = await _repository.GetByCustomerIdAsync(customerId);

		// Assert
		result.Should().HaveCount(3);
		result.Should().AllSatisfy(o => o.CustomerId.Should().Be(customerId));
	}

	[Fact]
	public async Task GetPagedAsync_ShouldReturnPagedResult()
	{
		// Arrange
		var customerId = await CreateTestCustomer();
		for (int i = 0; i < 15; i++)
		{
			await _repository.AddAsync(new Order(customerId));
		}

		// Act
		var result = await _repository.GetPagedAsync(1, 10);

		// Assert
		result.Items.Should().HaveCount(10);
		result.TotalCount.Should().Be(15);
	}

	[Fact]
	public async Task DeleteAsync_WhenOrderExists_ShouldReturnTrue()
	{
		// Arrange
		var customerId = await CreateTestCustomer();
		var order = new Order(customerId);
		var id = await _repository.AddAsync(order);

		// Act
		var result = await _repository.DeleteAsync(id);

		// Assert
		result.Should().BeTrue();
	}

	public void Dispose()
	{
		_fixture.ClearTables();
	}
}