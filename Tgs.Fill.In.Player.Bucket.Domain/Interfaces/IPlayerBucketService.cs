namespace Tgs.Fill.In.Player.Bucket.Domain.Interfaces;

public interface IPlayerBucketService
{
    Task<IEnumerable<BucketGameType>?> GetBrandBucketTypesAsync(Guid brandId, CancellationToken cancellationToken);

    Task FillInPlayerMainBucketAsync(Guid roundId, string playerCode, decimal bucketAmount, string remark,
        CancellationToken cancellationToken);

    Task FillInPlayerGameTypeBucketAsync(IEnumerable<string> gameTypes, Guid roundId, string playerCode,
        decimal bucketAmount, string remark, CancellationToken cancellationToken);
}