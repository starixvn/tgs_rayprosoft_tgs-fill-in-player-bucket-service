using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Tgs.Fill.In.Player.Bucket.Domain;
using Tgs.Fill.In.Player.Bucket.Repositories.Entities;

namespace Tgs.Fill.In.Player.Bucket.Repositories.DependencyInjection;

public static class TgsRepositoryExtensions
{
	public static IServiceCollection AddRepositories(this IServiceCollection services,
        Action<IServiceProvider, DbContextOptionsBuilder> dbContextConfigure)
    {
        services
            .AddDbContext<tgsContext>(dbContextConfigure)
            .AddTransient<ITgsRepository, TgsRepository>();

        return services;
	}
}
