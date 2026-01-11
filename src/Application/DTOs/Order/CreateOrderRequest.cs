namespace Application.DTOs.Order;

public record CreateOrderRequest(
	int CustomerId,
	List<CreateOrderItemRequest> Items
);

public record CreateOrderItemRequest(
	int ProductId,
	int Quantity,
	decimal UnitPrice
);
