using Dapper;
using Domain.Common;
using Domain.Entities;
using Domain.Interfaces;
using System.Data;

namespace Infrastructure.IntegrationTests.Repositories;

public class TestCustomerRepository : ICustomerRepository
{
	private readonly IDbConnection _connection;

	public TestCustomerRepository(IDbConnection connection)
	{
		_connection = connection;
	}

	public async Task<Customer?> GetByIdAsync(int id)
	{
		const string sql = "SELECT * FROM Customers WHERE Id = @Id";
		return await _connection.QueryFirstOrDefaultAsync<Customer>(sql, new { Id = id });
	}

	public async Task<Customer?> GetByEmailAsync(string email)
	{
		const string sql = "SELECT * FROM Customers WHERE Email = @Email";
		return await _connection.QueryFirstOrDefaultAsync<Customer>(sql, new { Email = email });
	}

	public async Task<IEnumerable<Customer>> GetAllAsync()
	{
		const string sql = "SELECT * FROM Customers";
		return await _connection.QueryAsync<Customer>(sql);
	}

	public async Task<PagedResult<Customer>> GetPagedAsync(int pageNumber, int pageSize)
	{
		const string countSql = "SELECT COUNT(*) FROM Customers";
		const string dataSql = """
            SELECT * FROM Customers
            ORDER BY Id
            LIMIT @PageSize OFFSET @Offset
            """;

		var totalCount = await _connection.ExecuteScalarAsync<int>(countSql);
		var offset = (pageNumber - 1) * pageSize;
		var items = await _connection.QueryAsync<Customer>(dataSql, new { Offset = offset, PageSize = pageSize });

		return new PagedResult<Customer>(items, pageNumber, pageSize, totalCount);
	}

	public async Task<int> AddAsync(Customer customer)
	{
		const string sql = """
            INSERT INTO Customers (Name, Email, CreatedAt)
            VALUES (@Name, @Email, @CreatedAt);
            SELECT last_insert_rowid();
            """;

		return await _connection.ExecuteScalarAsync<int>(sql, new
		{
			customer.Name,
			Email = customer.Email.Value,
			CreatedAt = customer.CreatedAt.ToString("o")
		});
	}

	public async Task<bool> UpdateAsync(Customer customer)
	{
		const string sql = """
            UPDATE Customers 
            SET Name = @Name, Email = @Email
            WHERE Id = @Id
            """;

		var affected = await _connection.ExecuteAsync(sql, new
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
		var affected = await _connection.ExecuteAsync(sql, new { Id = id });
		return affected > 0;
	}
}