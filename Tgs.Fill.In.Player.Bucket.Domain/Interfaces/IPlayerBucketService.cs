namespace Tgs.Fill.In.Player.Bucket.Domain.Interfaces;

public interface IPlayerBucketService
{
    Task<IEnumerable<BucketGameType>?> GetBrandBucketTypesAsync(Guid brandId, CancellationToken cancellationToken);

    Task FillInPlayerGameTypeBucketAsync(BucketGameType gameType, Guid roundId, string playerCode,
        decimal bucketAmount, string remark, CancellationToken cancellationToken);
}