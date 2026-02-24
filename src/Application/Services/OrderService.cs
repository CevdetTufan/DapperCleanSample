using Application.DTOs.Order;
using Application.Mappings;
using Domain.Common;
using Domain.Entities;
using Domain.Exceptions;
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
		return order?.ToDto();
	}

	public async Task<OrderDto?> GetByIdWithItemsAsync(int id)
	{
		var order = await _orderRepository.GetByIdWithItemsAsync(id);
		return order?.ToDto();
	}

	public async Task<IEnumerable<OrderDto>> GetByCustomerIdAsync(int customerId)
	{
		var orders = await _orderRepository.GetByCustomerIdAsync(customerId);
		return orders.Select(o => o.ToDto());
	}

	public async Task<PagedResult<OrderDto>> GetPagedAsync(int pageNumber, int pageSize)
	{
		var result = await _orderRepository.GetPagedAsync(pageNumber, pageSize);
		var dtos = result.Items.Select(o => o.ToDto());
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

	public async Task MarkAsPaidAsync(int id)
	{
		var order = await _orderRepository.GetByIdAsync(id)
			?? throw new NotFoundException(nameof(Order), id);

		try
		{
			order.MarkAsPaid();
			await _orderRepository.UpdateAsync(order);
		}
		catch (InvalidOperationException ex)
		{
			throw new BusinessRuleException("OrderStatus", ex.Message);
		}
	}

	public async Task ShipAsync(int id)
	{
		var order = await _orderRepository.GetByIdAsync(id)
			?? throw new NotFoundException(nameof(Order), id);

		try
		{
			order.Ship();
			await _orderRepository.UpdateAsync(order);
		}
		catch (InvalidOperationException ex)
		{
			throw new BusinessRuleException("OrderStatus", ex.Message);
		}
	}

	public async Task DeliverAsync(int id)
	{
		var order = await _orderRepository.GetByIdAsync(id)
			?? throw new NotFoundException(nameof(Order), id);

		try
		{
			order.Deliver();
			await _orderRepository.UpdateAsync(order);
		}
		catch (InvalidOperationException ex)
		{
			throw new BusinessRuleException("OrderStatus", ex.Message);
		}
	}

	public async Task CancelAsync(int id)
	{
		var order = await _orderRepository.GetByIdAsync(id)
			?? throw new NotFoundException(nameof(Order), id);

		try
		{
			order.Cancel();
			await _orderRepository.UpdateAsync(order);
		}
		catch (InvalidOperationException ex)
		{
			throw new BusinessRuleException("OrderStatus", ex.Message);
		}
	}

	public async Task DeleteAsync(int id)
	{
		var exists = await _orderRepository.GetByIdAsync(id)
			?? throw new NotFoundException(nameof(Order), id);

		await _orderItemRepository.DeleteByOrderIdAsync(id);
		await _orderRepository.DeleteAsync(id);
	}
}
