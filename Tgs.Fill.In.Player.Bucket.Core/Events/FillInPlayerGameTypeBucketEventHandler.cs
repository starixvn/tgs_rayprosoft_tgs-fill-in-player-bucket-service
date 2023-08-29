using MediatR;
using Microsoft.Extensions.Logging;
using Polly;
using Tgs.Fill.In.Player.Bucket.Domain.Events;
using Tgs.Fill.In.Player.Bucket.Domain.Interfaces;

namespace Tgs.Fill.In.Player.Bucket.Core.Events;

internal class FillInPlayerGameTypeBucketEventHandler : INotificationHandler<FillInPlayerBucketEvent>
{
    private readonly ILogger<FillInPlayerGameTypeBucketEventHandler> _logger;
    private readonly IPlayerBucketService _playerBucketService;

    public FillInPlayerGameTypeBucketEventHandler(ILogger<FillInPlayerGameTypeBucketEventHandler> logger,
        IPlayerBucketService playerBucketService)
    {
        _logger = logger;
        _playerBucketService = playerBucketService;
    }

    public async Task Handle(FillInPlayerBucketEvent @event, CancellationToken cancellationToken)
    {
        var playerBucketType = @event.PlayerRound.GetBucketGameType();

        _logger.LogInformation(
            "Fill player game bucket type {playerCode} was replaced ({gameType}) with ({gameBucketType}) {bucketAmount} bucket amount execute start time {StartTime}.",
            @event.PlayerRound.PlayerCode, @event.PlayerRound.GameType, playerBucketType,
            @event.PlayerRound.GetBucketAmount(), @event.StartDateTime);

        await Policy.Handle<Exception>()
            .RetryAsync(
                3,
                (exception, retryCount) => _logger.LogError(
                    "PlayerCode:{PlayerCode} BucketAmount:{BucketAmount} retryCount:{retryCount} Exception:{exception}",
                    @event.PlayerRound.PlayerCode,
                    @event.PlayerRound.GetBucketAmount(),
                    retryCount,
                    exception))
            .ExecuteAsync(
                async () =>
                    await _playerBucketService.FillInPlayerGameTypeBucketAsync(
                        playerBucketType,
                        @event.PlayerRound.RoundId,
                        @event.PlayerRound.PlayerCode,
                        @event.PlayerRound.GetBucketAmount(),
                        "",
                        cancellationToken).ConfigureAwait(false));
    }
}