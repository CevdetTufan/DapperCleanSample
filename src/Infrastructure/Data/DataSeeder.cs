using Bogus;
using Dapper;

namespace Infrastructure.Data;

public class DataSeeder
{
	private readonly DapperContext _context;

	internal DataSeeder(DapperContext context)
	{
		_context = context;
	}

	/// <summary>
	/// Seeds the database with fake data if it's empty.
	/// </summary>
	/// <returns>True if data was seeded, false if data already exists.</returns>
	public async Task<bool> SeedAsync(int customerCount = 10, int productCount = 20, int orderCount = 15)
	{
		using var connection = _context.CreateConnection();

		// Check if data already exists
		var existingCustomers = await connection.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM Customers");
		if (existingCustomers > 0)
		{
			return false; // Data already seeded, skip
		}

		// Seed Customers
		var customerIds = await SeedCustomersAsync(connection, customerCount);

		// Seed Products
		var productIds = await SeedProductsAsync(connection, productCount);

		// Seed Orders with Items
		await SeedOrdersAsync(connection, customerIds, productIds, orderCount);

		return true; // Data seeded successfully
	}

	private static async Task<List<int>> SeedCustomersAsync(System.Data.IDbConnection connection, int count)
	{
		var faker = new Faker<CustomerSeedModel>("tr")
			.RuleFor(c => c.Name, f => f.Name.FullName())
			.RuleFor(c => c.Email, f => f.Internet.Email())
			.RuleFor(c => c.CreatedAt, f => f.Date.Past(1).ToString("o"));

		var customers = faker.Generate(count);
		var ids = new List<int>();

		foreach (var customer in customers)
		{
			var id = await connection.ExecuteScalarAsync<int>(
				"""
				INSERT INTO Customers (Name, Email, CreatedAt)
				VALUES (@Name, @Email, @CreatedAt);
				SELECT last_insert_rowid();
				""",
				customer);
			ids.Add(id);
		}

		return ids;
	}

	private static async Task<List<int>> SeedProductsAsync(System.Data.IDbConnection connection, int count)
	{
		var productNames = new[]
		{
			"Laptop", "Mouse", "Keyboard", "Monitor", "Headphones",
			"Webcam", "USB Hub", "External SSD", "Graphics Card", "RAM",
			"Motherboard", "Power Supply", "PC Case", "CPU Cooler", "Thermal Paste",
			"HDMI Cable", "USB Cable", "Mouse Pad", "Desk Lamp", "Chair"
		};

		var faker = new Faker<ProductSeedModel>("tr")
			.RuleFor(p => p.Name, f => f.PickRandom(productNames) + " " + f.Commerce.ProductAdjective())
			.RuleFor(p => p.Price, f => Math.Round(f.Random.Decimal(50, 5000), 2))
			.RuleFor(p => p.CreatedAt, f => f.Date.Past(1).ToString("o"));

		var products = faker.Generate(count);
		var ids = new List<int>();

		foreach (var product in products)
		{
			var id = await connection.ExecuteScalarAsync<int>(
				"""
				INSERT INTO Products (Name, Price, CreatedAt)
				VALUES (@Name, @Price, @CreatedAt);
				SELECT last_insert_rowid();
				""",
				product);
			ids.Add(id);
		}

		return ids;
	}

	private static async Task SeedOrdersAsync(
		System.Data.IDbConnection connection,
		List<int> customerIds,
		List<int> productIds,
		int count)
	{
		var faker = new Faker("tr");
		var statuses = new[] { 0, 1, 2, 3 }; // Pending, Paid, Shipped, Delivered

		for (int i = 0; i < count; i++)
		{
			var customerId = faker.PickRandom(customerIds);
			var orderDate = faker.Date.Past(1).ToString("o");
			var status = faker.PickRandom(statuses);
			var createdAt = orderDate;

			var orderId = await connection.ExecuteScalarAsync<int>(
				"""
				INSERT INTO Orders (CustomerId, OrderDate, Status, CreatedAt)
				VALUES (@CustomerId, @OrderDate, @Status, @CreatedAt);
				SELECT last_insert_rowid();
				""",
				new { CustomerId = customerId, OrderDate = orderDate, Status = status, CreatedAt = createdAt });

			// Add 1-4 items per order
			var itemCount = faker.Random.Int(1, 4);
			var selectedProducts = faker.PickRandom(productIds, itemCount).ToList();

			foreach (var productId in selectedProducts)
			{
				var quantity = faker.Random.Int(1, 5);
				var unitPrice = Math.Round(faker.Random.Decimal(100, 2000), 2);

				await connection.ExecuteAsync(
					"""
					INSERT INTO OrderItems (OrderId, ProductId, Quantity, UnitPrice)
					VALUES (@OrderId, @ProductId, @Quantity, @UnitPrice);
					""",
					new { OrderId = orderId, ProductId = productId, Quantity = quantity, UnitPrice = unitPrice });
			}
		}
	}

	// Internal seed models (property-based records for Bogus Faker compatibility)
	private sealed record CustomerSeedModel
	{
		public string Name { get; set; } = null!;
		public string Email { get; set; } = null!;
		public string CreatedAt { get; set; } = null!;
	}

	private sealed record ProductSeedModel
	{
		public string Name { get; set; } = null!;
		public decimal Price { get; set; }= 0;
		public string CreatedAt { get; set; } = null!;
	}
}
