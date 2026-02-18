using Dapper;

namespace Infrastructure.Data;

public class DatabaseInitializer
{
	private readonly DapperContext _context;

	internal DatabaseInitializer(DapperContext context)
	{
		_context = context;
	}

	public async Task InitializeAsync()
	{
		using var connection = _context.CreateConnection();

		await connection.ExecuteAsync("""
			CREATE TABLE IF NOT EXISTS Customers (
			    Id INTEGER PRIMARY KEY AUTOINCREMENT,
			    Name TEXT NOT NULL,
			    Email TEXT NOT NULL UNIQUE,
			    CreatedAt TEXT NOT NULL
			);

			CREATE TABLE IF NOT EXISTS Products (
			    Id INTEGER PRIMARY KEY AUTOINCREMENT,
			    Name TEXT NOT NULL,
			    Price REAL NOT NULL,
			    CreatedAt TEXT NOT NULL
			);

			CREATE TABLE IF NOT EXISTS Orders (
			    Id INTEGER PRIMARY KEY AUTOINCREMENT,
			    CustomerId INTEGER NOT NULL,
			    OrderDate TEXT NOT NULL,
			    Status INTEGER NOT NULL,
			    CreatedAt TEXT NOT NULL,
			    FOREIGN KEY (CustomerId) REFERENCES Customers(Id)
			);

			CREATE TABLE IF NOT EXISTS OrderItems (
			    Id INTEGER PRIMARY KEY AUTOINCREMENT,
			    OrderId INTEGER NOT NULL,
			    ProductId INTEGER NOT NULL,
			    Quantity INTEGER NOT NULL,
			    UnitPrice REAL NOT NULL,
			    FOREIGN KEY (OrderId) REFERENCES Orders(Id),
			    FOREIGN KEY (ProductId) REFERENCES Products(Id)
			);
			""");
	}
}
