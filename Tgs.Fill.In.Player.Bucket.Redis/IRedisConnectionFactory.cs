using StackExchange.Redis;

namespace Tgs.Fill.In.Player.Bucket.Redis;

public interface IRedisConnectionFactory
{
	public IDatabase GetDataBase();
}
