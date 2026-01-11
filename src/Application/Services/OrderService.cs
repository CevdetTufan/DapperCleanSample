using Application.DTOs.Order;
using Domain.Common;
using Domain.Entities;
using Domain.Interfaces;

namespace Application.Services;

public class OrderService : IOrderService
{
	private readonly IOrderRepository _orderRepository;
	private readonly IOrderItemRepository _orderItemRepository;

	public OrderService(IOrderRepository orderRepository, IOrderItemRepository orderItemRepository)
	{
		_orderRepository = orderRepository;
		_orderItemRepository = orderItemRepository;
	}

	public async Task<OrderDto?> GetByIdAsync(int id)
	{
		var order = await _orderRepository.GetByIdAsync(id);
		return order is null ? null : MapToDto(order);
	}

	public async Task<OrderDto?> GetByIdWithItemsAsync(int id)
	{
		var order = await _orderRepository.GetByIdWithItemsAsync(id);
		return order is null ? null : MapToDto(order);
	}

	public async Task<IEnumerable<OrderDto>> GetByCustomerIdAsync(int customerId)
	{
		var orders = await _orderRepository.GetByCustomerIdAsync(customerId);
		return orders.Select(MapToDto);
	}

	public async Task<PagedResult<OrderDto>> GetPagedAsync(int pageNumber, int pageSize)
	{
		var result = await _orderRepository.GetPagedAsync(pageNumber, pageSize);
		var dtos = result.Items.Select(MapToDto);
		return new PagedResult<OrderDto>(dtos, pageNumber, pageSize, result.TotalCount);
	}

	public async Task<int> CreateAsync(CreateOrderRequest request)
	{
		var order = new Order(request.CustomerId);
		var orderId = await _orderRepository.AddAsync(order);

		foreach (var item in request.Items)
		{
			var orderItem = new OrderItem(orderId, item.ProductId, item.Quantity, item.UnitPrice);
			await _orderItemRepository.AddAsync(orderItem);
		}

		return orderId;
	}

	public async Task<bool> MarkAsPaidAsync(int id)
	{
		var order = await _orderRepository.GetByIdAsync(id);
		if (order is null)
			return false;

		order.MarkAsPaid();
		return await _orderRepository.UpdateAsync(order);
	}

	public async Task<bool> ShipAsync(int id)
	{
		var order = await _orderRepository.GetByIdAsync(id);
		if (order is null)
			return false;

		order.Ship();
		return await _orderRepository.UpdateAsync(order);
	}

	public async Task<bool> DeliverAsync(int id)
	{
		var order = await _orderRepository.GetByIdAsync(id);
		if (order is null)
			return false;

		order.Deliver();
		return await _orderRepository.UpdateAsync(order);
	}

	public async Task<bool> CancelAsync(int id)
	{
		var order = await _orderRepository.GetByIdAsync(id);
		if (order is null)
			return false;

		order.Cancel();
		return await _orderRepository.UpdateAsync(order);
	}

	public async Task<bool> DeleteAsync(int id)
	{
		await _orderItemRepository.DeleteByOrderIdAsync(id);
		return await _orderRepository.DeleteAsync(id);
	}

	private static OrderDto MapToDto(Order order)
	{
		var items = order.Items.Select(i => new OrderItemDto(
			i.Id,
			i.ProductId,
			i.Quantity,
			i.UnitPrice,
			i.TotalPrice
		));

		return new OrderDto(
			order.Id,
			order.CustomerId,
			order.OrderDate,
			order.Status,
			order.TotalAmount,
			order.CreatedAt,
			items
		);
	}
}
