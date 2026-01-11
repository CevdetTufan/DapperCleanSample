namespace Application.DTOs.Order;

public record OrderItemDto(
	int Id,
	int ProductId,
	int Quantity,
	decimal UnitPrice,
	decimal TotalPrice
);
