using Domain.Entities;
using FluentAssertions;

namespace Domain.UnitTests.Entities;

public class OrderItemTests
{
	[Fact]
	public void Constructor_WithValidParameters_ShouldCreateOrderItem()
	{
		// Arrange & Act
		var orderItem = new OrderItem(1, 1, 2, 100m);

		// Assert
		orderItem.OrderId.Should().Be(1);
		orderItem.ProductId.Should().Be(1);
		orderItem.Quantity.Should().Be(2);
		orderItem.UnitPrice.Should().Be(100m);
	}

	[Fact]
	public void TotalPrice_ShouldReturnQuantityTimesUnitPrice()
	{
		// Arrange
		var orderItem = new OrderItem(1, 1, 3, 50m);

		// Act
		var totalPrice = orderItem.TotalPrice;

		// Assert
		totalPrice.Should().Be(150m);
	}

	[Theory]
	[InlineData(0)]
	[InlineData(-1)]
	public void Constructor_WithInvalidProductId_ShouldThrowArgumentException(int invalidProductId)
	{
		// Act
		var act = () => new OrderItem(1, invalidProductId, 1, 100m);

		// Assert
		act.Should().Throw<ArgumentException>()
			.WithMessage("*ProductId must be positive*");
	}

	[Theory]
	[InlineData(0)]
	[InlineData(-1)]
	public void Constructor_WithInvalidQuantity_ShouldThrowArgumentException(int invalidQuantity)
	{
		// Act
		var act = () => new OrderItem(1, 1, invalidQuantity, 100m);

		// Assert
		act.Should().Throw<ArgumentException>()
			.WithMessage("*Quantity must be positive*");
	}

	[Fact]
	public void UpdateQuantity_WithValidQuantity_ShouldUpdateQuantity()
	{
		// Arrange
		var orderItem = new OrderItem(1, 1, 2, 100m);

		// Act
		orderItem.UpdateQuantity(5);

		// Assert
		orderItem.Quantity.Should().Be(5);
	}
}