using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Tgs.Fill.In.Player.Bucket.Client.DependencyInjection;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddFillInPlayerBucketClient(
		this IServiceCollection services,
		Action<FillInPlayerBucketOptions, IServiceProvider> configure)
		=> services
			.AddOptions<FillInPlayerBucketOptions>()
			.Configure(configure)
			.Services
			.AddHttpClient<IFillInPlayerBucketClient, FillInPlayerBucketClient>((sp, c) =>
			{
				var options = sp.GetRequiredService<IOptions<FillInPlayerBucketOptions>>().Value;

				c.BaseAddress = options.Url;
			}).Services;
}