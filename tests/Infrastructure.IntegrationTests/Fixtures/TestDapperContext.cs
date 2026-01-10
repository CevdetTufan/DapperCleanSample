using System.Data;

namespace Infrastructure.IntegrationTests.Fixtures;

public class TestDapperContext
{
	private readonly IDbConnection _connection;

	public TestDapperContext(IDbConnection connection)
	{
		_connection = connection;
	}

	public IDbConnection CreateConnection() => _connection;
}