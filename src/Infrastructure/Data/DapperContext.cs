using Microsoft.Data.Sqlite;
using System.Data;

namespace Infrastructure.Data;

internal class DapperContext
{
	private readonly string _connectionString;

	public DapperContext(string connectionString)
	{
		_connectionString = connectionString;
	}

	public IDbConnection CreateConnection() => new SqliteConnection(_connectionString);
}
