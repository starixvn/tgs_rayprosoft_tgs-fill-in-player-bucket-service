using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Polly;
using Tgs.Fill.In.Player.Bucket.Domain.Events;
using Tgs.Fill.In.Player.Bucket.Domain.Interfaces;
using Tgs.Fill.In.Player.Bucket.Redis;
using Tgs.Service.PlayerBuckets;

namespace Tgs.Fill.In.Player.Bucket.Core.Events
{
    internal class FillInPlayerGameTypeBucketEventHandler : INotificationHandler<FillInPlayerBucketEvent>
    {
        private readonly ILogger<FillInPlayerGameTypeBucketEventHandler> _logger;
        private readonly IPlayerBucketService _playerBucketService;

        public FillInPlayerGameTypeBucketEventHandler(ILogger<FillInPlayerGameTypeBucketEventHandler> logger, IPlayerBucketService playerBucketService)
        {
            _logger = logger;
            _playerBucketService = playerBucketService;
        }

        public async Task Handle(FillInPlayerBucketEvent @event, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Fill player game type bucket execute start time {request.StartTime}.", @event.StartDateTime);

            var bucketTypes = await _playerBucketService.GetBrandBucketTypesAsync(@event.PlayerRound.BrandId, cancellationToken);

            if (bucketTypes is not null && bucketTypes.Contains(@event.PlayerRound.GetBucketGameType()))
            {
                await Policy.Handle<Exception>()
                    .RetryAsync(
                        3,
                        (exception, retryCount) => _logger.LogError(
                            "PlayerCode:{model.PlayerCode} BucketAmount:{model.BucketAmount} retryCount:{retryCount} Exception:{exception}",
                            @event.PlayerRound.PlayerCode,
                            @event.PlayerRound.GetBucketAmount(),
                            retryCount,
                            exception))
                    .ExecuteAsync(
                        async () =>
                            await _playerBucketService.FillInPlayerGameTypeBucketAsync(
                                new[] { "" },
                                @event.PlayerRound.RoundId,
                                @event.PlayerRound.PlayerCode,
                                @event.PlayerRound.GetBucketAmount(),
                                "",
                                cancellationToken).ConfigureAwait(false));
            }
        }
    }
}
