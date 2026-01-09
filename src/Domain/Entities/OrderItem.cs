namespace Domain.Entities;

public class OrderItem
{
	public int Id { get; private set; }
	public int OrderId { get; private set; }
	public int ProductId { get; private set; }
	public int Quantity { get; private set; }
	public decimal UnitPrice { get; private set; }

	public decimal TotalPrice => Quantity * UnitPrice;

	private OrderItem() { }

	public OrderItem(int orderId, int productId, int quantity, decimal unitPrice)
	{
		if (orderId < 0)
			throw new ArgumentException("OrderId cannot be negative", nameof(orderId));

		if (productId <= 0)
			throw new ArgumentException("ProductId must be positive", nameof(productId));

		if (quantity <= 0)
			throw new ArgumentException("Quantity must be positive", nameof(quantity));

		if (unitPrice <= 0)
			throw new ArgumentException("UnitPrice must be positive", nameof(unitPrice));

		OrderId = orderId;
		ProductId = productId;
		Quantity = quantity;
		UnitPrice = unitPrice;
	}

	public void UpdateQuantity(int quantity)
	{
		if (quantity <= 0)
			throw new ArgumentException("Quantity must be positive", nameof(quantity));

		Quantity = quantity;
	}
}