using System.Data;
using Dapper;
using Microsoft.Data.Sqlite;

namespace Infrastructure.IntegrationTests.Fixtures;

public class DatabaseFixture : IDisposable
{
	public IDbConnection Connection { get; }
	private bool _disposed;

	public DatabaseFixture()
	{
		Connection = new SqliteConnection("DataSource=:memory:");
		Connection.Open();
		InitializeDatabase();
	}

	private void InitializeDatabase()
	{
		const string sql = """
            CREATE TABLE Products (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Name TEXT NOT NULL,
                Price REAL NOT NULL,
                CreatedAt TEXT NOT NULL
            );

            CREATE TABLE Customers (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Name TEXT NOT NULL,
                Email TEXT NOT NULL,
                CreatedAt TEXT NOT NULL
            );

            CREATE TABLE Orders (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                CustomerId INTEGER NOT NULL,
                OrderDate TEXT NOT NULL,
                Status INTEGER NOT NULL,
                CreatedAt TEXT NOT NULL,
                FOREIGN KEY (CustomerId) REFERENCES Customers(Id)
            );

            CREATE TABLE OrderItems (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                OrderId INTEGER NOT NULL,
                ProductId INTEGER NOT NULL,
                Quantity INTEGER NOT NULL,
                UnitPrice REAL NOT NULL,
                FOREIGN KEY (OrderId) REFERENCES Orders(Id),
                FOREIGN KEY (ProductId) REFERENCES Products(Id)
            );
            """;

		Connection.Execute(sql);
	}

	public void ClearTables()
	{
		Connection.Execute("DELETE FROM OrderItems");
		Connection.Execute("DELETE FROM Orders");
		Connection.Execute("DELETE FROM Products");
		Connection.Execute("DELETE FROM Customers");
	}

	protected virtual void Dispose(bool disposing)
	{
		if (!_disposed)
		{
			if (disposing)
			{
				Connection.Close();
				Connection.Dispose();
			}
			_disposed = true;
		}
	}

	public void Dispose()
	{
		Dispose(true);
		GC.SuppressFinalize(this);
	}
}
