using Dapper;
using Domain.Interfaces;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Infrastructure.TypeHandlers;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class DependencyInjection
{
	private static bool _typeHandlersRegistered;

	public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString)
	{
		RegisterTypeHandlers();

		services.AddSingleton(new DapperContext(connectionString));

		services.AddScoped<IProductRepository, ProductRepository>();
		services.AddScoped<ICustomerRepository, CustomerRepository>();
		services.AddScoped<IOrderRepository, OrderRepository>();
		services.AddScoped<IOrderItemRepository, OrderItemRepository>();

		return services;
	}

	private static void RegisterTypeHandlers()
	{
		if (!_typeHandlersRegistered)
		{
			SqlMapper.AddTypeHandler(new EmailTypeHandler());
			_typeHandlersRegistered = true;
		}
	}
}
