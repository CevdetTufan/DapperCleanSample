using Domain.Entities;
using FluentAssertions;

namespace Domain.UnitTests.Entities;

public class ProductTests
{
	[Fact]
	public void Constructor_WithValidParameters_ShouldCreateProduct()
	{
		// Arrange & Act
		var product = new Product("Laptop", 15000m);

		// Assert
		product.Name.Should().Be("Laptop");
		product.Price.Should().Be(15000m);
		product.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
	}

	[Theory]
	[InlineData("")]
	[InlineData(" ")]
	[InlineData(null)]
	public void Constructor_WithInvalidName_ShouldThrowArgumentException(string? invalidName)
	{
		// Arrange & Act
		var act = () => new Product(invalidName!, 100m);

		// Assert
		act.Should().Throw<ArgumentException>()
			.WithMessage("*Name cannot be empty*");
	}

	[Theory]
	[InlineData(0)]
	[InlineData(-1)]
	[InlineData(-100)]
	public void Constructor_WithInvalidPrice_ShouldThrowArgumentException(decimal invalidPrice)
	{
		// Arrange & Act
		var act = () => new Product("Laptop", invalidPrice);

		// Assert
		act.Should().Throw<ArgumentException>()
			.WithMessage("*Price must be positive*");
	}

	[Fact]
	public void Constructor_WithNameExceeding200Characters_ShouldThrowArgumentException()
	{
		// Arrange
		var longName = new string('a', 201);

		// Act
		var act = () => new Product(longName, 100m);

		// Assert
		act.Should().Throw<ArgumentException>()
			.WithMessage("*Name cannot exceed 200 characters*");
	}

	[Fact]
	public void UpdateName_WithValidName_ShouldUpdateName()
	{
		// Arrange
		var product = new Product("Laptop", 15000m);

		// Act
		product.UpdateName("Gaming Laptop");

		// Assert
		product.Name.Should().Be("Gaming Laptop");
	}

	[Fact]
	public void UpdatePrice_WithValidPrice_ShouldUpdatePrice()
	{
		// Arrange
		var product = new Product("Laptop", 15000m);

		// Act
		product.UpdatePrice(20000m);

		// Assert
		product.Price.Should().Be(20000m);
	}
}