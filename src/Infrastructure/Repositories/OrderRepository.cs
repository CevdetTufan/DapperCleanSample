using Dapper;
using Domain.Common;
using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Data;

namespace Infrastructure.Repositories;

internal class OrderRepository : IOrderRepository
{
	private readonly DapperContext _context;

	public OrderRepository(DapperContext context)
	{
		_context = context;
	}

	public async Task<Order?> GetByIdAsync(int id)
	{
		const string sql = "SELECT * FROM Orders WHERE Id = @Id";
		using var connection = _context.CreateConnection();
		return await connection.QueryFirstOrDefaultAsync<Order>(sql, new { Id = id });
	}

	public async Task<Order?> GetByIdWithItemsAsync(int id)
	{
		const string sql = """
            SELECT o.*, oi.*
            FROM Orders o
            LEFT JOIN OrderItems oi ON o.Id = oi.OrderId
            WHERE o.Id = @Id
            """;

		using var connection = _context.CreateConnection();
		var orderDictionary = new Dictionary<int, Order>();

		await connection.QueryAsync<Order, OrderItem, Order>(
			sql,
			(order, orderItem) =>
			{
				if (!orderDictionary.TryGetValue(order.Id, out var existingOrder))
				{
					existingOrder = order;
					orderDictionary.Add(order.Id, existingOrder);
				}

				return existingOrder;
			},
			new { Id = id },
			splitOn: "Id"
		);

		return orderDictionary.Values.FirstOrDefault();
	}

	public async Task<IEnumerable<Order>> GetByCustomerIdAsync(int customerId)
	{
		const string sql = "SELECT * FROM Orders WHERE CustomerId = @CustomerId ORDER BY OrderDate DESC";
		using var connection = _context.CreateConnection();
		return await connection.QueryAsync<Order>(sql, new { CustomerId = customerId });
	}

	public async Task<IEnumerable<Order>> GetAllAsync()
	{
		const string sql = "SELECT * FROM Orders ORDER BY OrderDate DESC";
		using var connection = _context.CreateConnection();
		return await connection.QueryAsync<Order>(sql);
	}

	public async Task<PagedResult<Order>> GetPagedAsync(int pageNumber, int pageSize)
	{
		const string countSql = "SELECT COUNT(*) FROM Orders";
		const string dataSql = """
            SELECT * FROM Orders
            ORDER BY OrderDate DESC
            OFFSET @Offset ROWS
            FETCH NEXT @PageSize ROWS ONLY
            """;

		using var connection = _context.CreateConnection();
		var totalCount = await connection.ExecuteScalarAsync<int>(countSql);
		var offset = (pageNumber - 1) * pageSize;
		var items = await connection.QueryAsync<Order>(dataSql, new { Offset = offset, PageSize = pageSize });

		return new PagedResult<Order>(items, pageNumber, pageSize, totalCount);
	}

	public async Task<PagedResult<Order>> GetPagedByCustomerIdAsync(int customerId, int pageNumber, int pageSize)
	{
		const string countSql = "SELECT COUNT(*) FROM Orders WHERE CustomerId = @CustomerId";
		const string dataSql = """
            SELECT * FROM Orders
            WHERE CustomerId = @CustomerId
            ORDER BY OrderDate DESC
            OFFSET @Offset ROWS
            FETCH NEXT @PageSize ROWS ONLY
            """;

		using var connection = _context.CreateConnection();
		var totalCount = await connection.ExecuteScalarAsync<int>(countSql, new { CustomerId = customerId });
		var offset = (pageNumber - 1) * pageSize;
		var items = await connection.QueryAsync<Order>(dataSql, new { CustomerId = customerId, Offset = offset, PageSize = pageSize });

		return new PagedResult<Order>(items, pageNumber, pageSize, totalCount);
	}

	public async Task<int> AddAsync(Order order)
	{
		const string sql = """
            INSERT INTO Orders (CustomerId, OrderDate, Status, CreatedAt)
            VALUES (@CustomerId, @OrderDate, @Status, @CreatedAt);
            SELECT CAST(SCOPE_IDENTITY() AS INT);
            """;

		using var connection = _context.CreateConnection();
		return await connection.ExecuteScalarAsync<int>(sql, order);
	}

	public async Task<bool> UpdateAsync(Order order)
	{
		const string sql = """
            UPDATE Orders 
            SET Status = @Status
            WHERE Id = @Id
            """;

		using var connection = _context.CreateConnection();
		var affected = await connection.ExecuteAsync(sql, order);
		return affected > 0;
	}

	public async Task<bool> DeleteAsync(int id)
	{
		const string sql = "DELETE FROM Orders WHERE Id = @Id";
		using var connection = _context.CreateConnection();
		var affected = await connection.ExecuteAsync(sql, new { Id = id });
		return affected > 0;
	}
}