using MediatR;
using Microsoft.Extensions.Logging;
using Polly;
using Tgs.Fill.In.Player.Bucket.Domain;
using Tgs.Fill.In.Player.Bucket.Domain.Events;
using Tgs.Fill.In.Player.Bucket.Domain.Models;
using Tgs.Service.PlayerBuckets;

namespace Tgs.Fill.In.Player.Bucket.Core
{
    internal class FillInPlayerBucketHandler : IRequestHandler<FillInPlayerBucketRequest>
    {
        private readonly ITgsRepository _tgsRepository;
        private readonly IPlayerBucketClient _playerBucketClient;
        private readonly ILogger<FillInPlayerBucketHandler> _logger;
        private readonly IMediator _mediator;

        public FillInPlayerBucketHandler(ITgsRepository tgsRepository, IPlayerBucketClient playerBucketClient, ILogger<FillInPlayerBucketHandler> logger, IMediator mediator)
        {
            _tgsRepository = tgsRepository;
            _playerBucketClient = playerBucketClient;
            _logger = logger;
            _mediator = mediator;
        }

        public async Task Handle(FillInPlayerBucketRequest request, CancellationToken cancellationToken)
        {
            var rounds = _tgsRepository.GetPlayerRoundsAsync(request.StartDateTime.UtcDateTime, request.StartDateTime.AddMinutes(1).UtcDateTime, cancellationToken);

            await rounds.ForEachAsync(
                round => _mediator.Publish(new AddRoundRecordEvent(round, request.StartDateTime.UtcDateTime),
                    cancellationToken), cancellationToken: cancellationToken);
        }
    }
}
