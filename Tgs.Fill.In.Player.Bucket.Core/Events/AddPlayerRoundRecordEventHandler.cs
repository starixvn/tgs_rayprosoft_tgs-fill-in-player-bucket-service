using MediatR;
using Tgs.Fill.In.Player.Bucket.Domain.Events;
using Tgs.Fill.In.Player.Bucket.Redis;

namespace Tgs.Fill.In.Player.Bucket.Core.Events;

internal class AddPlayerRoundRecordEventHandler : INotificationHandler<AddRoundRecordEvent>
{
    private readonly IMediator _mediator;
    private readonly IRedisConnectionFactory _redisConnectionFactory;

    public AddPlayerRoundRecordEventHandler(IRedisConnectionFactory redisConnectionFactory, IMediator mediator)
    {
        _redisConnectionFactory = redisConnectionFactory;
        _mediator = mediator;
    }

    public async Task Handle(AddRoundRecordEvent @event, CancellationToken cancellationToken)
    {
        var key = @event.PlayerRound.GetRedisKey(@event.StartDateTime);

        var value = @event.PlayerRound.GetRedisValue();

        var db = _redisConnectionFactory.GetDataBase();

        if (!await db.SetContainsAsync(key, value))
        {
            await db.SetAddAsync(key, value);

            await db.KeyExpireAsync(key, TimeSpan.FromDays(49));

            await _mediator.Publish(new FillInPlayerBucketEvent(@event.PlayerRound, @event.StartDateTime),
                cancellationToken);
        }
    }
}