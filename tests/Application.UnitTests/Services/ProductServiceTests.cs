using Application.DTOs.Product;
using Application.Services;
using Domain.Common;
using Domain.Entities;
using Domain.Interfaces;
using FluentAssertions;
using NSubstitute;

namespace Application.UnitTests.Services;

public class ProductServiceTests
{
	private readonly IProductRepository _productRepository;
	private readonly ProductService _sut;

	public ProductServiceTests()
	{
		_productRepository = Substitute.For<IProductRepository>();
		_sut = new ProductService(_productRepository);
	}

	[Fact]
	public async Task GetByIdAsync_WhenProductExists_ReturnsProductDto()
	{
		// Arrange
		var product = new Product("Test Product", 99.99m);
		_productRepository.GetByIdAsync(1).Returns(product);

		// Act
		var result = await _sut.GetByIdAsync(1);

		// Assert
		result.Should().NotBeNull();
		result!.Name.Should().Be("Test Product");
		result.Price.Should().Be(99.99m);
	}

	[Fact]
	public async Task GetByIdAsync_WhenProductNotExists_ReturnsNull()
	{
		// Arrange
		_productRepository.GetByIdAsync(1).Returns((Product?)null);

		// Act
		var result = await _sut.GetByIdAsync(1);

		// Assert
		result.Should().BeNull();
	}

	[Fact]
	public async Task GetAllAsync_ReturnsAllProducts()
	{
		// Arrange
		var products = new List<Product>
		{
			new("Product 1", 10.00m),
			new("Product 2", 20.00m),
			new("Product 3", 30.00m)
		};
		_productRepository.GetAllAsync().Returns(products);

		// Act
		var result = await _sut.GetAllAsync();

		// Assert
		result.Should().HaveCount(3);
	}

	[Fact]
	public async Task GetPagedAsync_ReturnsPagedResult()
	{
		// Arrange
		var products = new List<Product>
		{
			new("Product 1", 10.00m),
			new("Product 2", 20.00m)
		};
		var pagedResult = new PagedResult<Product>(products, 1, 10, 2);
		_productRepository.GetPagedAsync(1, 10).Returns(pagedResult);

		// Act
		var result = await _sut.GetPagedAsync(1, 10);

		// Assert
		result.Items.Should().HaveCount(2);
		result.PageNumber.Should().Be(1);
		result.PageSize.Should().Be(10);
		result.TotalCount.Should().Be(2);
	}

	[Fact]
	public async Task CreateAsync_ValidRequest_ReturnsNewId()
	{
		// Arrange
		var request = new CreateProductRequest("New Product", 49.99m);
		_productRepository.AddAsync(Arg.Any<Product>()).Returns(1);

		// Act
		var result = await _sut.CreateAsync(request);

		// Assert
		result.Should().Be(1);
		await _productRepository.Received(1).AddAsync(Arg.Is<Product>(p =>
			p.Name == "New Product" && p.Price == 49.99m));
	}

	[Fact]
	public async Task UpdateAsync_WhenProductExists_ReturnsTrue()
	{
		// Arrange
		var product = new Product("Old Product", 10.00m);
		_productRepository.GetByIdAsync(1).Returns(product);
		_productRepository.UpdateAsync(Arg.Any<Product>()).Returns(true);

		var request = new UpdateProductRequest("Updated Product", 25.00m);

		// Act
		var result = await _sut.UpdateAsync(1, request);

		// Assert
		result.Should().BeTrue();
		await _productRepository.Received(1).UpdateAsync(Arg.Is<Product>(p =>
			p.Name == "Updated Product" && p.Price == 25.00m));
	}

	[Fact]
	public async Task UpdateAsync_WhenProductNotExists_ReturnsFalse()
	{
		// Arrange
		_productRepository.GetByIdAsync(1).Returns((Product?)null);
		var request = new UpdateProductRequest("Updated Product", 25.00m);

		// Act
		var result = await _sut.UpdateAsync(1, request);

		// Assert
		result.Should().BeFalse();
		await _productRepository.DidNotReceive().UpdateAsync(Arg.Any<Product>());
	}

	[Fact]
	public async Task DeleteAsync_WhenProductExists_ReturnsTrue()
	{
		// Arrange
		_productRepository.DeleteAsync(1).Returns(true);

		// Act
		var result = await _sut.DeleteAsync(1);

		// Assert
		result.Should().BeTrue();
		await _productRepository.Received(1).DeleteAsync(1);
	}

	[Fact]
	public async Task DeleteAsync_WhenProductNotExists_ReturnsFalse()
	{
		// Arrange
		_productRepository.DeleteAsync(1).Returns(false);

		// Act
		var result = await _sut.DeleteAsync(1);

		// Assert
		result.Should().BeFalse();
	}
}
