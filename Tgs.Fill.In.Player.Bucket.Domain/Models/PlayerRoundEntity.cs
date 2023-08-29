namespace Tgs.Fill.In.Player.Bucket.Domain.Models;

public class PlayerRoundEntity
{
    public Guid BrandId { get; set; }
    public Guid RoundId { get; set; }
    public Guid GameProviderId { get; set; }
    public int GameId { get; set; }
    public GameType GameType { get; set; }
    public string PlayerCode { get; set; } = null!;
    public string ExternalRoundId { get; set; } = null!;
    public decimal Turnover { get; set; }
    public decimal EffectiveBettingPercentage { get; set; }

    public decimal GetBucketAmount() => Turnover * EffectiveBettingPercentage / 100;

    public string GetRedisKey(DateTime startDateTime) => $"fillIn:{startDateTime:d}:{GameId}:{PlayerCode}";

    public string GetRedisValue() => ExternalRoundId;

    public BucketGameType GetBucketGameType() => Consts.GameTypeGroups[GameType];
}