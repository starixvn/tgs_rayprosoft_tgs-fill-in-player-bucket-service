using System.Collections.Immutable;

namespace Tgs.Fill.In.Player.Bucket.Domain;

public static class Consts
{
    public static GameType[] SlotGroup =
    {
        GameType.Fishing,
        GameType.FishingLobby,
        GameType.Arcade,
        GameType.ArcadeLobby,
        GameType.Slot,
        GameType.TableGame
    };

    public static GameType[] SportsGroup =
    {
        GameType.Sportsbook,
        GameType.ESports
    };

    public static GameType[] VirtualSportsGroup =
    {
        GameType.VirtualSports,
        GameType.VirtualSportsLobby
    };

    public static GameType[] LiveDealerGroup =
    {
        GameType.LiveDealer,
        GameType.LiveDealerLobby
    };

    public static GameType[] CardGameGroup =
    {
        GameType.CardGame,
        GameType.CardGameLobby
    };

    public static GameType[] LotteryGroup =
    {
        GameType.Lottery,
        GameType.LotteryLobby
    };

    public static IImmutableDictionary<GameType, BucketGameType> GameTypeGroups
        => new Dictionary<GameType, BucketGameType>
        {
            {GameType.Sportsbook, BucketGameType.Sportsbook}
        }.ToImmutableDictionary();
}