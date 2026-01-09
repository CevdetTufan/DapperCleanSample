using Dapper;
using Domain.Common;
using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Data;

namespace Infrastructure.Repositories;

internal class ProductRepository : IProductRepository
{
	private readonly DapperContext _context;

	public ProductRepository(DapperContext context)
	{
		_context = context;
	}

	public async Task<int> AddAsync(Product product)
	{
		const string sql = """
            INSERT INTO Products (Name, Price, CreatedAt)
            VALUES (@Name, @Price, @CreatedAt);
            SELECT CAST(SCOPE_IDENTITY() AS INT);
            """;

		using var connection = _context.CreateConnection();
		return await connection.ExecuteScalarAsync<int>(sql, product);
	}

	public async Task<bool> DeleteAsync(int id)
	{
		const string sql = "DELETE FROM Products WHERE Id = @Id";
		using var connection = _context.CreateConnection();
		var affected = await connection.ExecuteAsync(sql, new { Id = id });
		return affected > 0;
	}

	public async Task<IEnumerable<Product>> GetAllAsync()
	{
		const string sql = "SELECT * FROM Products";
		using var connection = _context.CreateConnection();
		return await connection.QueryAsync<Product>(sql);
	}

	public async Task<Product?> GetByIdAsync(int id)
	{
		const string sql = "SELECT * FROM Products WHERE Id = @Id";
		using var connection = _context.CreateConnection();
		return await connection.QueryFirstOrDefaultAsync<Product>(sql, new { Id = id });
	}

	public async Task<PagedResult<Product>> GetPagedAsync(int pageNumber, int pageSize)
	{
		const string countSql = "SELECT COUNT(*) FROM Products";
		const string dataSql = """
            SELECT * FROM Products
            ORDER BY Id
            OFFSET @Offset ROWS
            FETCH NEXT @PageSize ROWS ONLY
            """;

		using var connection = _context.CreateConnection();
		var totalCount = await connection.ExecuteScalarAsync<int>(countSql);
		var offset = (pageNumber - 1) * pageSize;
		var items = await connection.QueryAsync<Product>(dataSql, new { Offset = offset, PageSize = pageSize });

		return new PagedResult<Product>(items, pageNumber, pageSize, totalCount);
	}

	public async Task<bool> UpdateAsync(Product product)
	{
		const string sql = """
            UPDATE Products 
            SET Name = @Name, Price = @Price
            WHERE Id = @Id
            """;

		using var connection = _context.CreateConnection();
		var affected = await connection.ExecuteAsync(sql, product);
		return affected > 0;
	}
}
