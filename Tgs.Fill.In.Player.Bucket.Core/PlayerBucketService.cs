using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Tgs.Fill.In.Player.Bucket.Domain;
using Tgs.Fill.In.Player.Bucket.Domain.Interfaces;
using Tgs.PlayerBucket.Client;
using Tgs.Service.PlayerBuckets;

namespace Tgs.Fill.In.Player.Bucket.Core
{
    internal class PlayerBucketService : IPlayerBucketService
    {
        private readonly IPlayerBucketClient _playerBucketClient;
        private readonly IPlayerBucketForTypesClient _playerBucketForTypesClient;
        private readonly IMemoryCache _memoryCache;

        public PlayerBucketService(IPlayerBucketClient playerBucketClient, IMemoryCache memoryCache, IPlayerBucketForTypesClient playerBucketForTypesClient)
        {
            _playerBucketClient = playerBucketClient;
            _memoryCache = memoryCache;
            _playerBucketForTypesClient = playerBucketForTypesClient;
        }
        public async Task<IEnumerable<BucketGameType>?> GetBrandBucketTypesAsync(Guid brandId,
            CancellationToken cancellationToken)
        {
            return await _memoryCache.GetOrCreateAsync(
                $"brandBucketType:{brandId}",
                async cacheEntry =>
                {
                    _ = cacheEntry.SetSlidingExpiration(TimeSpan.FromHours(1));
                    cacheEntry.AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1);

                    var bucketTypes = _playerBucketClient.GetBrandBucketTypesAsync(brandId, cancellationToken);

                    return await bucketTypes.Select(typeString => (BucketGameType)Enum.Parse(typeof(BucketGameType), typeString))
                        .ToArrayAsync(cancellationToken);
                });
        }

        public async Task FillInPlayerMainBucketAsync(Guid roundId, string playerCode, decimal bucketAmount, string remark, CancellationToken cancellationToken)
        {
            await _playerBucketClient.FillInBucketAsync(
                playerCode,
                bucketAmount,
                FillType.Game,
                roundId.ToString(),
                remark,
                cancellationToken).ConfigureAwait(false);
        }

        public async Task FillInPlayerGameTypeBucketAsync(IEnumerable<string> gameTypes, Guid roundId, string playerCode, decimal bucketAmount, string remark, CancellationToken cancellationToken)
        {
            await _playerBucketForTypesClient.FillInBucketForBucketTypesAsync(
                playerCode,
                bucketAmount,
                FillType.Game,
                roundId.ToString(),
                remark,
                gameTypes,
                cancellationToken).ConfigureAwait(false);
        }
    }
}
