using Tgs.Fill.In.Player.Bucket.Domain.Models;

namespace Tgs.Fill.In.Player.Bucket.Domain;

public interface ITgsRepository
{
    IAsyncEnumerable<PlayerRoundEntity> GetPlayerRoundsAsync(DateTime from, DateTime to, CancellationToken cancellationToken = default);


}