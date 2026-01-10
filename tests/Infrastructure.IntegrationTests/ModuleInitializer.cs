using System.Runtime.CompilerServices;
using Dapper;
using Infrastructure.IntegrationTests.Fixtures;

namespace Infrastructure.IntegrationTests;

public static class ModuleInitializer
{
	[ModuleInitializer]
	public static void Initialize()
	{
		SqlMapper.AddTypeHandler(new EmailTypeHandler());
	}
}
