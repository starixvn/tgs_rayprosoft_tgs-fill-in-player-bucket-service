using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Tgs.Fill.In.Player.Bucket.Core.DependencyInjection;

public static class CoreExtensions
{
	public static IServiceCollection AddCore(this IServiceCollection services)
	{
        services.AddMediatR(r => r.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

		return services;
	}
}
