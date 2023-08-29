using Microsoft.Extensions.Caching.Memory;
using Tgs.Fill.In.Player.Bucket.Domain;
using Tgs.Fill.In.Player.Bucket.Domain.Interfaces;
using Tgs.PlayerBucket.Client;
using Tgs.Service.PlayerBuckets;

namespace Tgs.Fill.In.Player.Bucket.Core;

internal class PlayerBucketService : IPlayerBucketService
{
    private readonly IMemoryCache _memoryCache;
    private readonly IPlayerBucketClient _playerBucketClient;
    private readonly IPlayerBucketForTypesClient _playerBucketForTypesClient;

    public PlayerBucketService(IPlayerBucketClient playerBucketClient, IMemoryCache memoryCache,
        IPlayerBucketForTypesClient playerBucketForTypesClient)
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
                _ = cacheEntry.SetAbsoluteExpiration(TimeSpan.FromDays(1));

                var bucketTypes = _playerBucketClient.GetBrandBucketTypesAsync(brandId, cancellationToken);

                return await bucketTypes.Select(typeString
                        => (BucketGameType)Enum.Parse(typeof(BucketGameType), typeString))
                    .ToArrayAsync(cancellationToken);
            });
    }

    public async Task FillInPlayerGameTypeBucketAsync(BucketGameType gameType, Guid roundId, string playerCode,
        decimal bucketAmount, string remark, CancellationToken cancellationToken)
    {
        var bucketTypes = await GetBrandBucketTypesAsync(Guid.NewGuid(), cancellationToken);

        await _playerBucketForTypesClient.FillInBucketForBucketTypesAsync(
            playerCode,
            bucketAmount,
            FillType.Game,
            roundId.ToString(),
            remark,
            bucketTypes?.Intersect(new[]{gameType}).Select(t => t.ToString()),
            cancellationToken).ConfigureAwait(false);
    }
}