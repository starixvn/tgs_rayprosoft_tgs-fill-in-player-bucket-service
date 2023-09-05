using Google.Protobuf.WellKnownTypes;
using MediatR;
using Microsoft.Extensions.Logging;
using Tgs.Fill.In.Player.Bucket.Domain;
using Tgs.Fill.In.Player.Bucket.Domain.Events;
using Tgs.Fill.In.Player.Bucket.Redis;

namespace Tgs.Fill.In.Player.Bucket.Core;

internal class FillInPlayerBucketHandler : IRequestHandler<FillInPlayerBucketRequest>
{
    private readonly ILogger<FillInPlayerBucketHandler> _logger;
    private readonly IMediator _mediator;
    private readonly IRedisConnectionFactory _redisConnectionFactory;
    private readonly ITgsRepository _tgsRepository;

    public FillInPlayerBucketHandler(
        ITgsRepository tgsRepository, 
        IMediator mediator,
        IRedisConnectionFactory redisConnectionFactory,
        ILogger<FillInPlayerBucketHandler> logger)
    {
        _tgsRepository = tgsRepository;
        _logger = logger;
        _redisConnectionFactory = redisConnectionFactory;
        _mediator = mediator;
    }

    public async Task Handle(FillInPlayerBucketRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fill player bucket execute start time {request.StartTime}.", request.StartDateTime);
        var db = _redisConnectionFactory.GetDataBase();

        var key = $"fill:in:player:bucket:{request.StartDateTime.Date:yyyyMMdd}";

        if (!await db.SetContainsAsync(key, request.StartDateTime.ToTimestamp().Seconds))
        {
            await db.SetAddAsync(key, request.StartDateTime.ToTimestamp().Seconds);

            await db.KeyExpireAsync(key, TimeSpan.FromDays(7));

            var rounds = _tgsRepository.GetPlayerRoundsAsync(request.StartDateTime, request.StartDateTime.AddMinutes(1),
                cancellationToken);

            await rounds.ForEachAsync(
                round => _mediator.Publish(new AddRoundRecordEvent(round, request.StartDateTime),
                    cancellationToken), cancellationToken);
        }
    }
}