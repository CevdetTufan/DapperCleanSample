using System.Data;
using Dapper;
using Domain.Common;
using Domain.Entities;
using Domain.Interfaces;

namespace Infrastructure.IntegrationTests.Repositories;

public class TestOrderRepository : IOrderRepository
{
	private readonly IDbConnection _connection;

	public TestOrderRepository(IDbConnection connection)
	{
		_connection = connection;
	}

	public async Task<Order?> GetByIdAsync(int id)
	{
		const string sql = "SELECT * FROM Orders WHERE Id = @Id";
		return await _connection.QueryFirstOrDefaultAsync<Order>(sql, new { Id = id });
	}

	public async Task<Order?> GetByIdWithItemsAsync(int id)
	{
		// Simplified for testing
		return await GetByIdAsync(id);
	}

	public async Task<IEnumerable<Order>> GetByCustomerIdAsync(int customerId)
	{
		const string sql = "SELECT * FROM Orders WHERE CustomerId = @CustomerId ORDER BY OrderDate DESC";
		return await _connection.QueryAsync<Order>(sql, new { CustomerId = customerId });
	}

	public async Task<IEnumerable<Order>> GetAllAsync()
	{
		const string sql = "SELECT * FROM Orders ORDER BY OrderDate DESC";
		return await _connection.QueryAsync<Order>(sql);
	}

	public async Task<PagedResult<Order>> GetPagedAsync(int pageNumber, int pageSize)
	{
		const string countSql = "SELECT COUNT(*) FROM Orders";
		const string dataSql = """
            SELECT * FROM Orders
            ORDER BY Id
            LIMIT @PageSize OFFSET @Offset
            """;

		var totalCount = await _connection.ExecuteScalarAsync<int>(countSql);
		var offset = (pageNumber - 1) * pageSize;
		var items = await _connection.QueryAsync<Order>(dataSql, new { Offset = offset, PageSize = pageSize });

		return new PagedResult<Order>(items, pageNumber, pageSize, totalCount);
	}

	public async Task<PagedResult<Order>> GetPagedByCustomerIdAsync(int customerId, int pageNumber, int pageSize)
	{
		const string countSql = "SELECT COUNT(*) FROM Orders WHERE CustomerId = @CustomerId";
		const string dataSql = """
            SELECT * FROM Orders
            WHERE CustomerId = @CustomerId
            ORDER BY Id
            LIMIT @PageSize OFFSET @Offset
            """;

		var totalCount = await _connection.ExecuteScalarAsync<int>(countSql, new { CustomerId = customerId });
		var offset = (pageNumber - 1) * pageSize;
		var items = await _connection.QueryAsync<Order>(dataSql, new { CustomerId = customerId, Offset = offset, PageSize = pageSize });

		return new PagedResult<Order>(items, pageNumber, pageSize, totalCount);
	}

	public async Task<int> AddAsync(Order order)
	{
		const string sql = """
            INSERT INTO Orders (CustomerId, OrderDate, Status, CreatedAt)
            VALUES (@CustomerId, @OrderDate, @Status, @CreatedAt);
            SELECT last_insert_rowid();
            """;

		return await _connection.ExecuteScalarAsync<int>(sql, new
		{
			order.CustomerId,
			OrderDate = order.OrderDate.ToString("o"),
			Status = (int)order.Status,
			CreatedAt = order.CreatedAt.ToString("o")
		});
	}

	public async Task<bool> UpdateAsync(Order order)
	{
		const string sql = """
            UPDATE Orders 
            SET Status = @Status
            WHERE Id = @Id
            """;

		var affected = await _connection.ExecuteAsync(sql, new { order.Id, Status = (int)order.Status });
		return affected > 0;
	}

	public async Task<bool> DeleteAsync(int id)
	{
		const string sql = "DELETE FROM Orders WHERE Id = @Id";
		var affected = await _connection.ExecuteAsync(sql, new { Id = id });
		return affected > 0;
	}
}