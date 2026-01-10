using Domain.Entities;
using FluentAssertions;
using Infrastructure.IntegrationTests.Fixtures;

namespace Infrastructure.IntegrationTests.Repositories;

public class ProductRepositoryTests : IClassFixture<DatabaseFixture>, IDisposable
{
	private readonly DatabaseFixture _fixture;
	private readonly TestProductRepository _repository;

	public ProductRepositoryTests(DatabaseFixture fixture)
	{
		_fixture = fixture;
		_repository = new TestProductRepository(_fixture.Connection);
		_fixture.ClearTables();
	}

	[Fact]
	public async Task AddAsync_ShouldInsertProductAndReturnId()
	{
		// Arrange
		var product = new Product("Laptop", 15000m);

		// Act
		var id = await _repository.AddAsync(product);

		// Assert
		id.Should().BeGreaterThan(0);
	}

	[Fact]
	public async Task GetByIdAsync_WhenProductExists_ShouldReturnProduct()
	{
		// Arrange
		var product = new Product("Laptop", 15000m);
		var id = await _repository.AddAsync(product);

		// Act
		var result = await _repository.GetByIdAsync(id);

		// Assert
		result.Should().NotBeNull();
		result!.Name.Should().Be("Laptop");
		result.Price.Should().Be(15000m);
	}

	[Fact]
	public async Task GetByIdAsync_WhenProductDoesNotExist_ShouldReturnNull()
	{
		// Act
		var result = await _repository.GetByIdAsync(999);

		// Assert
		result.Should().BeNull();
	}

	[Fact]
	public async Task GetAllAsync_ShouldReturnAllProducts()
	{
		// Arrange
		await _repository.AddAsync(new Product("Laptop", 15000m));
		await _repository.AddAsync(new Product("Mouse", 500m));
		await _repository.AddAsync(new Product("Keyboard", 1000m));

		// Act
		var result = await _repository.GetAllAsync();

		// Assert
		result.Should().HaveCount(3);
	}

	[Fact]
	public async Task GetPagedAsync_ShouldReturnPagedResult()
	{
		// Arrange
		for (int i = 1; i <= 25; i++)
		{
			await _repository.AddAsync(new Product($"Product {i}", i * 100m));
		}

		// Act
		var result = await _repository.GetPagedAsync(2, 10);

		// Assert
		result.Items.Should().HaveCount(10);
		result.PageNumber.Should().Be(2);
		result.PageSize.Should().Be(10);
		result.TotalCount.Should().Be(25);
		result.TotalPages.Should().Be(3);
		result.HasPreviousPage.Should().BeTrue();
		result.HasNextPage.Should().BeTrue();
	}

	[Fact]
	public async Task UpdateAsync_ShouldUpdateProduct()
	{
		// Arrange
		var product = new Product("Laptop", 15000m);
		var id = await _repository.AddAsync(product);
		var savedProduct = await _repository.GetByIdAsync(id);

		// Act - SQLite için direkt SQL ile güncelleme
		var updated = await _repository.UpdateByIdAsync(id, "Gaming Laptop", 20000m);

		// Assert
		updated.Should().BeTrue();
		var result = await _repository.GetByIdAsync(id);
		result!.Name.Should().Be("Gaming Laptop");
		result.Price.Should().Be(20000m);
	}

	[Fact]
	public async Task DeleteAsync_WhenProductExists_ShouldReturnTrue()
	{
		// Arrange
		var product = new Product("Laptop", 15000m);
		var id = await _repository.AddAsync(product);

		// Act
		var result = await _repository.DeleteAsync(id);

		// Assert
		result.Should().BeTrue();
		var deleted = await _repository.GetByIdAsync(id);
		deleted.Should().BeNull();
	}

	[Fact]
	public async Task DeleteAsync_WhenProductDoesNotExist_ShouldReturnFalse()
	{
		// Act
		var result = await _repository.DeleteAsync(999);

		// Assert
		result.Should().BeFalse();
	}

	public void Dispose()
	{
		_fixture.ClearTables();
	}
}