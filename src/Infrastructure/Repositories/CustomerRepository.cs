using Dapper;
using Domain.Common;
using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Data;

namespace Infrastructure.Repositories;

internal class CustomerRepository : ICustomerRepository
{
	private readonly DapperContext _context;

	public CustomerRepository(DapperContext context)
	{
		_context = context;
	}

	public async Task<Customer?> GetByIdAsync(int id)
	{
		const string sql = "SELECT * FROM Customers WHERE Id = @Id";
		using var connection = _context.CreateConnection();
		return await connection.QueryFirstOrDefaultAsync<Customer>(sql, new { Id = id });
	}

	public async Task<Customer?> GetByEmailAsync(string email)
	{
		const string sql = "SELECT * FROM Customers WHERE Email = @Email";
		using var connection = _context.CreateConnection();
		return await connection.QueryFirstOrDefaultAsync<Customer>(sql, new { Email = email });
	}

	public async Task<IEnumerable<Customer>> GetAllAsync()
	{
		const string sql = "SELECT * FROM Customers";
		using var connection = _context.CreateConnection();
		return await connection.QueryAsync<Customer>(sql);
	}

	public async Task<PagedResult<Customer>> GetPagedAsync(int pageNumber, int pageSize)
	{
		const string countSql = "SELECT COUNT(*) FROM Customers";
		const string dataSql = """
            SELECT * FROM Customers
            ORDER BY Id
            OFFSET @Offset ROWS
            FETCH NEXT @PageSize ROWS ONLY
            """;

		using var connection = _context.CreateConnection();
		var totalCount = await connection.ExecuteScalarAsync<int>(countSql);
		var offset = (pageNumber - 1) * pageSize;
		var items = await connection.QueryAsync<Customer>(dataSql, new { Offset = offset, PageSize = pageSize });

		return new PagedResult<Customer>(items, pageNumber, pageSize, totalCount);
	}

	public async Task<int> AddAsync(Customer customer)
	{
		const string sql = """
            INSERT INTO Customers (Name, Email, CreatedAt)
            VALUES (@Name, @Email, @CreatedAt);
            SELECT CAST(SCOPE_IDENTITY() AS INT);
            """;

		using var connection = _context.CreateConnection();
		return await connection.ExecuteScalarAsync<int>(sql, new
		{
			customer.Name,
			Email = customer.Email.Value,
			customer.CreatedAt
		});
	}

	public async Task<bool> UpdateAsync(Customer customer)
	{
		const string sql = """
            UPDATE Customers 
            SET Name = @Name, Email = @Email
            WHERE Id = @Id
            """;

		using var connection = _context.CreateConnection();
		var affected = await connection.ExecuteAsync(sql, new
		{
			customer.Id,
			customer.Name,
			Email = customer.Email.Value
		});
		return affected > 0;
	}

	public async Task<bool> DeleteAsync(int id)
	{
		const string sql = "DELETE FROM Customers WHERE Id = @Id";
		using var connection = _context.CreateConnection();
		var affected = await connection.ExecuteAsync(sql, new { Id = id });
		return affected > 0;
	}
}