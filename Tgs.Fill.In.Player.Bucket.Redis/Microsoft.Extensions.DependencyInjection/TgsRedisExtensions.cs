using Microsoft.Extensions.DependencyInjection;

namespace Tgs.Fill.In.Player.Bucket.Redis.Microsoft.Extensions.DependencyInjection;

public static class TgsRedisExtensions
{
    public static IServiceCollection AddRedis(
		this IServiceCollection service,
		Action<RedisOptions, IServiceProvider> configure)
	{
		_ = service
			.AddOptions<RedisOptions>()
			.Configure(configure)
			.Services
			.AddSingleton<IRedisConnectionFactory, RedisConnectionFactory>();

		return service;
	}
}
