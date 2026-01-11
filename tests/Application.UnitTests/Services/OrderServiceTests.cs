using Application.DTOs.Order;
using Application.Services;
using Domain.Common;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using FluentAssertions;
using NSubstitute;

namespace Application.UnitTests.Services;

public class OrderServiceTests
{
	private readonly IOrderRepository _orderRepository;
	private readonly IOrderItemRepository _orderItemRepository;
	private readonly OrderService _sut;

	public OrderServiceTests()
	{
		_orderRepository = Substitute.For<IOrderRepository>();
		_orderItemRepository = Substitute.For<IOrderItemRepository>();
		_sut = new OrderService(_orderRepository, _orderItemRepository);
	}

	[Fact]
	public async Task GetByIdAsync_WhenOrderExists_ReturnsOrderDto()
	{
		// Arrange
		var order = new Order(1);
		_orderRepository.GetByIdAsync(1).Returns(order);

		// Act
		var result = await _sut.GetByIdAsync(1);

		// Assert
		result.Should().NotBeNull();
		result!.CustomerId.Should().Be(1);
		result.Status.Should().Be(OrderStatus.Pending);
	}

	[Fact]
	public async Task GetByIdAsync_WhenOrderNotExists_ReturnsNull()
	{
		// Arrange
		_orderRepository.GetByIdAsync(1).Returns((Order?)null);

		// Act
		var result = await _sut.GetByIdAsync(1);

		// Assert
		result.Should().BeNull();
	}

	[Fact]
	public async Task GetByIdWithItemsAsync_WhenOrderExists_ReturnsOrderDtoWithItems()
	{
		// Arrange
		var order = new Order(1);
		_orderRepository.GetByIdWithItemsAsync(1).Returns(order);

		// Act
		var result = await _sut.GetByIdWithItemsAsync(1);

		// Assert
		result.Should().NotBeNull();
		result!.CustomerId.Should().Be(1);
	}

	[Fact]
	public async Task GetByIdWithItemsAsync_WhenOrderNotExists_ReturnsNull()
	{
		// Arrange
		_orderRepository.GetByIdWithItemsAsync(1).Returns((Order?)null);

		// Act
		var result = await _sut.GetByIdWithItemsAsync(1);

		// Assert
		result.Should().BeNull();
	}

	[Fact]
	public async Task GetByCustomerIdAsync_ReturnsCustomerOrders()
	{
		// Arrange
		var orders = new List<Order>
		{
			new(1),
			new(1)
		};
		_orderRepository.GetByCustomerIdAsync(1).Returns(orders);

		// Act
		var result = await _sut.GetByCustomerIdAsync(1);

		// Assert
		result.Should().HaveCount(2);
		result.Should().AllSatisfy(o => o.CustomerId.Should().Be(1));
	}

	[Fact]
	public async Task GetPagedAsync_ReturnsPagedResult()
	{
		// Arrange
		var orders = new List<Order>
		{
			new(1),
			new(2)
		};
		var pagedResult = new PagedResult<Order>(orders, 1, 10, 2);
		_orderRepository.GetPagedAsync(1, 10).Returns(pagedResult);

		// Act
		var result = await _sut.GetPagedAsync(1, 10);

		// Assert
		result.Items.Should().HaveCount(2);
		result.PageNumber.Should().Be(1);
		result.PageSize.Should().Be(10);
		result.TotalCount.Should().Be(2);
	}

	[Fact]
	public async Task CreateAsync_ValidRequest_ReturnsNewIdAndCreatesItems()
	{
		// Arrange
		var request = new CreateOrderRequest(1, new List<CreateOrderItemRequest>
		{
			new(1, 2, 10.00m),
			new(2, 1, 20.00m)
		});
		_orderRepository.AddAsync(Arg.Any<Order>()).Returns(1);
		_orderItemRepository.AddAsync(Arg.Any<OrderItem>()).Returns(1);

		// Act
		var result = await _sut.CreateAsync(request);

		// Assert
		result.Should().Be(1);
		await _orderRepository.Received(1).AddAsync(Arg.Is<Order>(o => o.CustomerId == 1));
		await _orderItemRepository.Received(2).AddAsync(Arg.Any<OrderItem>());
	}

	[Fact]
	public async Task CreateAsync_EmptyItems_ReturnsNewIdWithoutCreatingItems()
	{
		// Arrange
		var request = new CreateOrderRequest(1, new List<CreateOrderItemRequest>());
		_orderRepository.AddAsync(Arg.Any<Order>()).Returns(1);

		// Act
		var result = await _sut.CreateAsync(request);

		// Assert
		result.Should().Be(1);
		await _orderRepository.Received(1).AddAsync(Arg.Any<Order>());
		await _orderItemRepository.DidNotReceive().AddAsync(Arg.Any<OrderItem>());
	}

	[Fact]
	public async Task MarkAsPaidAsync_WhenOrderExists_ReturnsTrue()
	{
		// Arrange
		var order = new Order(1);
		_orderRepository.GetByIdAsync(1).Returns(order);
		_orderRepository.UpdateAsync(Arg.Any<Order>()).Returns(true);

		// Act
		var result = await _sut.MarkAsPaidAsync(1);

		// Assert
		result.Should().BeTrue();
		await _orderRepository.Received(1).UpdateAsync(Arg.Is<Order>(o => o.Status == OrderStatus.Paid));
	}

	[Fact]
	public async Task MarkAsPaidAsync_WhenOrderNotExists_ReturnsFalse()
	{
		// Arrange
		_orderRepository.GetByIdAsync(1).Returns((Order?)null);

		// Act
		var result = await _sut.MarkAsPaidAsync(1);

		// Assert
		result.Should().BeFalse();
		await _orderRepository.DidNotReceive().UpdateAsync(Arg.Any<Order>());
	}

	[Fact]
	public async Task ShipAsync_WhenOrderExistsAndPaid_ReturnsTrue()
	{
		// Arrange
		var order = new Order(1);
		order.MarkAsPaid();
		_orderRepository.GetByIdAsync(1).Returns(order);
		_orderRepository.UpdateAsync(Arg.Any<Order>()).Returns(true);

		// Act
		var result = await _sut.ShipAsync(1);

		// Assert
		result.Should().BeTrue();
		await _orderRepository.Received(1).UpdateAsync(Arg.Is<Order>(o => o.Status == OrderStatus.Shipped));
	}

	[Fact]
	public async Task ShipAsync_WhenOrderNotExists_ReturnsFalse()
	{
		// Arrange
		_orderRepository.GetByIdAsync(1).Returns((Order?)null);

		// Act
		var result = await _sut.ShipAsync(1);

		// Assert
		result.Should().BeFalse();
		await _orderRepository.DidNotReceive().UpdateAsync(Arg.Any<Order>());
	}

	[Fact]
	public async Task DeliverAsync_WhenOrderExistsAndShipped_ReturnsTrue()
	{
		// Arrange
		var order = new Order(1);
		order.MarkAsPaid();
		order.Ship();
		_orderRepository.GetByIdAsync(1).Returns(order);
		_orderRepository.UpdateAsync(Arg.Any<Order>()).Returns(true);

		// Act
		var result = await _sut.DeliverAsync(1);

		// Assert
		result.Should().BeTrue();
		await _orderRepository.Received(1).UpdateAsync(Arg.Is<Order>(o => o.Status == OrderStatus.Delivered));
	}

	[Fact]
	public async Task DeliverAsync_WhenOrderNotExists_ReturnsFalse()
	{
		// Arrange
		_orderRepository.GetByIdAsync(1).Returns((Order?)null);

		// Act
		var result = await _sut.DeliverAsync(1);

		// Assert
		result.Should().BeFalse();
		await _orderRepository.DidNotReceive().UpdateAsync(Arg.Any<Order>());
	}

	[Fact]
	public async Task CancelAsync_WhenOrderExistsAndPending_ReturnsTrue()
	{
		// Arrange
		var order = new Order(1);
		_orderRepository.GetByIdAsync(1).Returns(order);
		_orderRepository.UpdateAsync(Arg.Any<Order>()).Returns(true);

		// Act
		var result = await _sut.CancelAsync(1);

		// Assert
		result.Should().BeTrue();
		await _orderRepository.Received(1).UpdateAsync(Arg.Is<Order>(o => o.Status == OrderStatus.Cancelled));
	}

	[Fact]
	public async Task CancelAsync_WhenOrderNotExists_ReturnsFalse()
	{
		// Arrange
		_orderRepository.GetByIdAsync(1).Returns((Order?)null);

		// Act
		var result = await _sut.CancelAsync(1);

		// Assert
		result.Should().BeFalse();
		await _orderRepository.DidNotReceive().UpdateAsync(Arg.Any<Order>());
	}

	[Fact]
	public async Task DeleteAsync_DeletesOrderAndItems()
	{
		// Arrange
		_orderItemRepository.DeleteByOrderIdAsync(1).Returns(true);
		_orderRepository.DeleteAsync(1).Returns(true);

		// Act
		var result = await _sut.DeleteAsync(1);

		// Assert
		result.Should().BeTrue();
		await _orderItemRepository.Received(1).DeleteByOrderIdAsync(1);
		await _orderRepository.Received(1).DeleteAsync(1);
	}

	[Fact]
	public async Task DeleteAsync_WhenOrderNotExists_ReturnsFalse()
	{
		// Arrange
		_orderItemRepository.DeleteByOrderIdAsync(1).Returns(true);
		_orderRepository.DeleteAsync(1).Returns(false);

		// Act
		var result = await _sut.DeleteAsync(1);

		// Assert
		result.Should().BeFalse();
	}
}
