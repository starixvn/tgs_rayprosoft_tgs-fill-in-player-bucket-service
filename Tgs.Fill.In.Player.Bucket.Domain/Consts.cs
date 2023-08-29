using System.Collections.Immutable;

namespace Tgs.Fill.In.Player.Bucket.Domain;

public static class Consts
{
    public static IImmutableDictionary<GameType, BucketGameType> GameTypeGroups
        => new Dictionary<GameType, BucketGameType>
        {
            {GameType.Sportsbook, BucketGameType.Sportsbook},
            {GameType.ESports, BucketGameType.Sportsbook},
            {GameType.VirtualSports, BucketGameType.VirtualSports},
            {GameType.VirtualSportsLobby, BucketGameType.VirtualSports},
            {GameType.LiveDealer, BucketGameType.LiveDealer},
            {GameType.LiveDealerLobby, BucketGameType.LiveDealer},
            {GameType.CardGame, BucketGameType.CardGame},
            {GameType.CardGameLobby, BucketGameType.CardGame},
            {GameType.Lottery, BucketGameType.Lottery},
            {GameType.LotteryLobby, BucketGameType.Lottery},
            {GameType.Slot, BucketGameType.Slot},
            {GameType.Fishing, BucketGameType.Slot},
            {GameType.Arcade, BucketGameType.Slot},
            {GameType.ArcadeLobby, BucketGameType.Slot},
            {GameType.TableGame, BucketGameType.Slot}
        }.ToImmutableDictionary();
}