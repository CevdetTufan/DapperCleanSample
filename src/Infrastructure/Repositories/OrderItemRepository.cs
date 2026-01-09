using Dapper;
using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Data;

namespace Infrastructure.Repositories;

internal class OrderItemRepository : IOrderItemRepository
{
	private readonly DapperContext _context;

	public OrderItemRepository(DapperContext context)
	{
		_context = context;
	}

	public async Task<OrderItem?> GetByIdAsync(int id)
	{
		const string sql = "SELECT * FROM OrderItems WHERE Id = @Id";
		using var connection = _context.CreateConnection();
		return await connection.QueryFirstOrDefaultAsync<OrderItem>(sql, new { Id = id });
	}

	public async Task<IEnumerable<OrderItem>> GetByOrderIdAsync(int orderId)
	{
		const string sql = "SELECT * FROM OrderItems WHERE OrderId = @OrderId";
		using var connection = _context.CreateConnection();
		return await connection.QueryAsync<OrderItem>(sql, new { OrderId = orderId });
	}

	public async Task<int> AddAsync(OrderItem orderItem)
	{
		const string sql = """
            INSERT INTO OrderItems (OrderId, ProductId, Quantity, UnitPrice)
            VALUES (@OrderId, @ProductId, @Quantity, @UnitPrice);
            SELECT CAST(SCOPE_IDENTITY() AS INT);
            """;

		using var connection = _context.CreateConnection();
		return await connection.ExecuteScalarAsync<int>(sql, orderItem);
	}

	public async Task<bool> UpdateAsync(OrderItem orderItem)
	{
		const string sql = """
            UPDATE OrderItems 
            SET Quantity = @Quantity, UnitPrice = @UnitPrice
            WHERE Id = @Id
            """;

		using var connection = _context.CreateConnection();
		var affected = await connection.ExecuteAsync(sql, orderItem);
		return affected > 0;
	}

	public async Task<bool> DeleteAsync(int id)
	{
		const string sql = "DELETE FROM OrderItems WHERE Id = @Id";
		using var connection = _context.CreateConnection();
		var affected = await connection.ExecuteAsync(sql, new { Id = id });
		return affected > 0;
	}

	public async Task<bool> DeleteByOrderIdAsync(int orderId)
	{
		const string sql = "DELETE FROM OrderItems WHERE OrderId = @OrderId";
		using var connection = _context.CreateConnection();
		var affected = await connection.ExecuteAsync(sql, new { OrderId = orderId });
		return affected > 0;
	}
}