using Domain.Entities;
using Domain.Enums;
using FluentAssertions;

namespace Domain.UnitTests.Entities;

public class OrderTests
{
	[Fact]
	public void Constructor_WithValidCustomerId_ShouldCreateOrder()
	{
		// Arrange & Act
		var order = new Order(1);

		// Assert
		order.CustomerId.Should().Be(1);
		order.Status.Should().Be(OrderStatus.Pending);
		order.OrderDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
	}

	[Theory]
	[InlineData(0)]
	[InlineData(-1)]
	public void Constructor_WithInvalidCustomerId_ShouldThrowArgumentException(int invalidId)
	{
		// Act
		var act = () => new Order(invalidId);

		// Assert
		act.Should().Throw<ArgumentException>()
			.WithMessage("*CustomerId must be positive*");
	}

	[Fact]
	public void MarkAsPaid_WhenPending_ShouldChangeStatusToPaid()
	{
		// Arrange
		var order = new Order(1);

		// Act
		order.MarkAsPaid();

		// Assert
		order.Status.Should().Be(OrderStatus.Paid);
	}

	[Fact]
	public void MarkAsPaid_WhenNotPending_ShouldThrowInvalidOperationException()
	{
		// Arrange
		var order = new Order(1);
		order.MarkAsPaid();

		// Act
		var act = () => order.MarkAsPaid();

		// Assert
		act.Should().Throw<InvalidOperationException>()
			.WithMessage("*Only pending orders can be paid*");
	}

	[Fact]
	public void Ship_WhenPaid_ShouldChangeStatusToShipped()
	{
		// Arrange
		var order = new Order(1);
		order.MarkAsPaid();

		// Act
		order.Ship();

		// Assert
		order.Status.Should().Be(OrderStatus.Shipped);
	}

	[Fact]
	public void Ship_WhenNotPaid_ShouldThrowInvalidOperationException()
	{
		// Arrange
		var order = new Order(1);

		// Act
		var act = () => order.Ship();

		// Assert
		act.Should().Throw<InvalidOperationException>()
			.WithMessage("*Only paid orders can be shipped*");
	}

	[Fact]
	public void Deliver_WhenShipped_ShouldChangeStatusToDelivered()
	{
		// Arrange
		var order = new Order(1);
		order.MarkAsPaid();
		order.Ship();

		// Act
		order.Deliver();

		// Assert
		order.Status.Should().Be(OrderStatus.Delivered);
	}

	[Fact]
	public void Cancel_WhenPending_ShouldChangeStatusToCancelled()
	{
		// Arrange
		var order = new Order(1);

		// Act
		order.Cancel();

		// Assert
		order.Status.Should().Be(OrderStatus.Cancelled);
	}

	[Fact]
	public void Cancel_WhenShipped_ShouldThrowInvalidOperationException()
	{
		// Arrange
		var order = new Order(1);
		order.MarkAsPaid();
		order.Ship();

		// Act
		var act = () => order.Cancel();

		// Assert
		act.Should().Throw<InvalidOperationException>()
			.WithMessage("*Cannot cancel shipped or delivered orders*");
	}
}