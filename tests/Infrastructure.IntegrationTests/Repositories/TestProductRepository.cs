using System.Data;
using Dapper;
using Domain.Common;
using Domain.Entities;
using Domain.Interfaces;

namespace Infrastructure.IntegrationTests.Repositories;

public class TestProductRepository : IProductRepository
{
	private readonly IDbConnection _connection;

	public TestProductRepository(IDbConnection connection)
	{
		_connection = connection;
	}

	public async Task<Product?> GetByIdAsync(int id)
	{
		const string sql = "SELECT * FROM Products WHERE Id = @Id";
		return await _connection.QueryFirstOrDefaultAsync<Product>(sql, new { Id = id });
	}

	public async Task<IEnumerable<Product>> GetAllAsync()
	{
		const string sql = "SELECT * FROM Products";
		return await _connection.QueryAsync<Product>(sql);
	}

	public async Task<PagedResult<Product>> GetPagedAsync(int pageNumber, int pageSize)
	{
		const string countSql = "SELECT COUNT(*) FROM Products";
		const string dataSql = """
            SELECT * FROM Products
            ORDER BY Id
            LIMIT @PageSize OFFSET @Offset
            """;

		var totalCount = await _connection.ExecuteScalarAsync<int>(countSql);
		var offset = (pageNumber - 1) * pageSize;
		var items = await _connection.QueryAsync<Product>(dataSql, new { Offset = offset, PageSize = pageSize });

		return new PagedResult<Product>(items, pageNumber, pageSize, totalCount);
	}

	public async Task<int> AddAsync(Product product)
	{
		const string sql = """
            INSERT INTO Products (Name, Price, CreatedAt)
            VALUES (@Name, @Price, @CreatedAt);
            SELECT last_insert_rowid();
            """;

		return await _connection.ExecuteScalarAsync<int>(sql, new
		{
			product.Name,
			product.Price,
			CreatedAt = product.CreatedAt.ToString("o")
		});
	}

	public async Task<bool> UpdateAsync(Product product)
	{
		const string sql = """
            UPDATE Products 
            SET Name = @Name, Price = @Price
            WHERE Id = @Id
            """;

		var affected = await _connection.ExecuteAsync(sql, product);
		return affected > 0;
	}

	public async Task<bool> UpdateByIdAsync(int id, string name, decimal price)
	{
		const string sql = """
            UPDATE Products 
            SET Name = @Name, Price = @Price
            WHERE Id = @Id
            """;

		var affected = await _connection.ExecuteAsync(sql, new { Id = id, Name = name, Price = price });
		return affected > 0;
	}

	public async Task<bool> DeleteAsync(int id)
	{
		const string sql = "DELETE FROM Products WHERE Id = @Id";
		var affected = await _connection.ExecuteAsync(sql, new { Id = id });
		return affected > 0;
	}
}