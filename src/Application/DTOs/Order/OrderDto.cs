using Domain.Enums;

namespace Application.DTOs.Order;

public record OrderDto(
	int Id,
	int CustomerId,
	DateTime OrderDate,
	OrderStatus Status,
	decimal TotalAmount,
	DateTime CreatedAt,
	IEnumerable<OrderItemDto> Items
);
