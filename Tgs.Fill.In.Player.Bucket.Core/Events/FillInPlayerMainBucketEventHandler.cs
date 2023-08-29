using MediatR;
using Microsoft.Extensions.Logging;
using Polly;
using Tgs.Fill.In.Player.Bucket.Domain.Events;
using Tgs.Fill.In.Player.Bucket.Domain.Interfaces;

namespace Tgs.Fill.In.Player.Bucket.Core.Events
{
    internal class FillInPlayerMainBucketEventHandler : INotificationHandler<FillInPlayerBucketEvent>
    {
        private readonly ILogger<FillInPlayerMainBucketEventHandler> _logger;
        private readonly IPlayerBucketService _playerBucketService;

        public FillInPlayerMainBucketEventHandler(ILogger<FillInPlayerMainBucketEventHandler> logger, IPlayerBucketService playerBucketService)
        {
            _logger = logger;
            _playerBucketService = playerBucketService;
        }

        public async Task Handle(FillInPlayerBucketEvent @event, CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "Fill player main bucket {playerCode} {bucketAmount} amount execute start time {request.StartTime}.",
                @event.PlayerRound.PlayerCode, @event.PlayerRound.GetBucketAmount(), @event.StartDateTime);

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
                        await _playerBucketService.FillInPlayerMainBucketAsync(
                            @event.PlayerRound.RoundId,
                            @event.PlayerRound.PlayerCode,
                            @event.PlayerRound.GetBucketAmount(),
                            "",
                            cancellationToken).ConfigureAwait(false));
        }
    }
}
