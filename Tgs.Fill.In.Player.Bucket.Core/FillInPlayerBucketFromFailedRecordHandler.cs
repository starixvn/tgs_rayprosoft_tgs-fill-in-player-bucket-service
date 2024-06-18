using MediatR;
using System.Text.Json;
using Tgs.Fill.In.Player.Bucket.Domain.Events;
using Tgs.Fill.In.Player.Bucket.Redis;

namespace Tgs.Fill.In.Player.Bucket.Core;

internal class FillInPlayerBucketFromFailedRecordHandler : IRequestHandler<FillInPlayerBucketFromFailedRecordRequest>
{
    private readonly IMediator _mediator;
    private readonly IRedisConnectionFactory _redisConnectionFactory;

    public FillInPlayerBucketFromFailedRecordHandler(
        IMediator mediator,
        IRedisConnectionFactory redisConnectionFactory)
    {
        _redisConnectionFactory = redisConnectionFactory;
        _mediator = mediator;
    }

    public async Task Handle(FillInPlayerBucketFromFailedRecordRequest request, CancellationToken cancellationToken)
    {
        var db = _redisConnectionFactory.GetDataBase();

        const string key = "fill:in:player:bucket:error";

        var count = await db.ListLengthAsync(key);

        for (var i = 0; i < count; i++)
        {
            var list = await db.ListLeftPopAsync(key, 1);

            if (list.Length <= 0) continue;

            var failedEvent = JsonSerializer.Deserialize<FillInPlayerBucketFailedEvent>(list[0].ToString());

            if (failedEvent != null)
            {
                await _mediator.Publish(
                    new FillInPlayerBucketEvent(failedEvent.PlayerRound,
                        failedEvent.StartDateTime),
                    cancellationToken);
            }
        }
    }
}