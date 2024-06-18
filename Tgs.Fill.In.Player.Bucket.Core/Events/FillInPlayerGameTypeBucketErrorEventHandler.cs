using System.Text.Json;
using MediatR;
using Tgs.Fill.In.Player.Bucket.Domain.Events;
using Tgs.Fill.In.Player.Bucket.Redis;

namespace Tgs.Fill.In.Player.Bucket.Core.Events;

internal class FillInPlayerGameTypeBucketErrorEventHandler : INotificationHandler<FillInPlayerBucketFailedEvent>
{
    private readonly IRedisConnectionFactory _redisConnectionFactory;

    public FillInPlayerGameTypeBucketErrorEventHandler(IRedisConnectionFactory redisConnectionFactory)
    {
        _redisConnectionFactory = redisConnectionFactory;
    }

    public async Task Handle(FillInPlayerBucketFailedEvent @event, CancellationToken cancellationToken)
    {
        var db = _redisConnectionFactory.GetDataBase();

        await db.ListRightPushAsync("fill:in:player:bucket:error", JsonSerializer.Serialize(@event));
    }
}