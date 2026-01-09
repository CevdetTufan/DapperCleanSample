namespace Domain.Entities;

public class Product
{
	public int Id { get; private set; }
	public string Name { get; private set; } = null!;
	public decimal Price { get; private set; }
	public DateTime CreatedAt { get; private set; }

	private Product() { }

	public Product(string name, decimal price)
	{
		SetName(name);
		SetPrice(price);
		CreatedAt = DateTime.UtcNow;
	}

	public void UpdateName(string name) => SetName(name);

	public void UpdatePrice(decimal price) => SetPrice(price);

	private void SetName(string name)
	{
		if (string.IsNullOrWhiteSpace(name))
			throw new ArgumentException("Name cannot be empty", nameof(name));

		if (name.Length > 200)
			throw new ArgumentException("Name cannot exceed 200 characters", nameof(name));

		Name = name;
	}

	private void SetPrice(decimal price)
	{
		if (price <= 0)
			throw new ArgumentException("Price must be positive", nameof(price));

		Price = price;
	}
}