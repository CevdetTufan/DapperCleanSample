using Domain.Enums;

namespace Domain.Entities;

public class Order
{
	public int Id { get; private set; }
	public int CustomerId { get; private set; }
	public DateTime OrderDate { get; private set; }
	public OrderStatus Status { get; private set; }
	public DateTime CreatedAt { get; private set; }

	private readonly List<OrderItem> _items = [];
	public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();

	private Order() { }

	public Order(int customerId)
	{
		if (customerId <= 0)
			throw new ArgumentException("CustomerId must be positive", nameof(customerId));

		CustomerId = customerId;
		OrderDate = DateTime.UtcNow;
		Status = OrderStatus.Pending;
		CreatedAt = DateTime.UtcNow;
	}

	public void AddItem(int productId, int quantity, decimal unitPrice)
	{
		if (Status != OrderStatus.Pending)
			throw new InvalidOperationException("Cannot modify a non-pending order");

		var item = new OrderItem(Id, productId, quantity, unitPrice);
		_items.Add(item);
	}

	public void RemoveItem(int productId)
	{
		if (Status != OrderStatus.Pending)
			throw new InvalidOperationException("Cannot modify a non-pending order");

		var item = _items.FirstOrDefault(x => x.ProductId == productId);
		if (item is not null)
			_items.Remove(item);
	}

	public decimal TotalAmount => _items.Sum(x => x.TotalPrice);

	public void MarkAsPaid()
	{
		if (Status != OrderStatus.Pending)
			throw new InvalidOperationException("Only pending orders can be paid");

		Status = OrderStatus.Paid;
	}

	public void Cancel()
	{
		if (Status is OrderStatus.Shipped or OrderStatus.Delivered)
			throw new InvalidOperationException("Cannot cancel shipped or delivered orders");

		Status = OrderStatus.Cancelled;
	}

	public void Ship()
	{
		if (Status != OrderStatus.Paid)
			throw new InvalidOperationException("Only paid orders can be shipped");

		Status = OrderStatus.Shipped;
	}

	public void Deliver()
	{
		if (Status != OrderStatus.Shipped)
			throw new InvalidOperationException("Only shipped orders can be delivered");

		Status = OrderStatus.Delivered;
	}
}