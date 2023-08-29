using System.Runtime.CompilerServices;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Tgs.Fill.In.Player.Bucket.Domain;
using Tgs.Fill.In.Player.Bucket.Domain.Models;
using Tgs.Fill.In.Player.Bucket.Repositories.Entities;

namespace Tgs.Fill.In.Player.Bucket.Repositories;

public class TgsRepository : ITgsRepository
{
    private readonly tgsContext _context;

    public TgsRepository(tgsContext context)
    {
        _context = context;
    }

    public async IAsyncEnumerable<PlayerRoundEntity> GetPlayerRoundsAsync(DateTime from, DateTime to,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        const string spName = @"
		WITH rounds AS(
		select
        r.brand_id,
		r.id round_id,
        r.external_round_id,
        r.game_provider_id,
        r.game_id,
        g.type game_type,
		r.player_code,
		r.turnover,
		coalesce(bg.effective_betting_percentage, 100.0) AS effective_betting_percentage
		from bet.rounds r
        join dbo.games g on g.id = r.game_id
		left join dbo.brand_games bg on bg.brand_id = r.brand_id and bg.game_id = r.game_id
		where r.closed_on >= @from and r.closed_on < @to and r.reopened_on is null
		and r.effective_betting_percentage is null
		),
		update_rounds as (
		  UPDATE bet.rounds r SET effective_betting_percentage = br.effective_betting_percentage
		  FROM rounds br where r.id = br.round_id
		)

		select
        brand_id BrandId,
        round_id RoundId,
        game_provider_id GameProviderId,
        game_id GameId,
        game_type GameType,
        external_round_id ExternalRoundId,
		player_code PlayerCode,
		turnover Turnover,
		effective_betting_percentage EffectiveBettingPercentage
		from rounds;"
            ;

        var conn = _context.Database.GetDbConnection();
        var transaction = _context.Database.CurrentTransaction?.GetDbTransaction();

        var response = await conn.ExecuteReaderAsync(spName,
            new Dictionary<string, object>
            {
                { "from", from },
                { "to", to }
            }, transaction);

        var rowParser = response.GetRowParser<PlayerRoundEntity>();

        while (await response.ReadAsync(cancellationToken).ConfigureAwait(false)) yield return rowParser(response);
    }
}