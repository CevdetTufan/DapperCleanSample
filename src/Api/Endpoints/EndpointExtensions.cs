using System.Reflection;

namespace Api.Endpoints;

public static class EndpointExtensions
{
	public static IEndpointRouteBuilder MapEndpoints(this IEndpointRouteBuilder app)
	{
		var endpointTypes = Assembly.GetExecutingAssembly()
			.GetTypes()
			.Where(t => t.IsClass && !t.IsAbstract && typeof(IEndpoint).IsAssignableFrom(t));

		foreach (var type in endpointTypes)
		{
			var endpoint = (IEndpoint)Activator.CreateInstance(type)!;
			endpoint.MapEndpoint(app);
		}

		return app;
	}
}
