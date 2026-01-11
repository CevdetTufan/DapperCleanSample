using Application.DTOs.Order;
using Domain.Entities;

namespace Application.Mappings;

public static class OrderMappingExtensions
{
	public static OrderDto ToDto(this Order order)
	{
		var items = order.Items.Select(i => i.ToDto());

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

	public static OrderItemDto ToDto(this OrderItem item)
	{
		return new OrderItemDto(
			item.Id,
			item.ProductId,
			item.Quantity,
			item.UnitPrice,
			item.TotalPrice
		);
	}
}
