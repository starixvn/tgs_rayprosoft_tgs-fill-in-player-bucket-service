using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Tgs.Fill.In.Player.Bucket.Core.Events;
using Tgs.Fill.In.Player.Bucket.Domain;
using Tgs.Fill.In.Player.Bucket.Domain.Events;
using Tgs.Fill.In.Player.Bucket.Domain.Interfaces;

namespace Tgs.Fill.In.Player.Bucket.Core.UnitTest;

public class FillInPlayerGameTypeBucketEventTests
{
    [Test]
    public async Task Fill_In_Player_Game_Type_Bucket()
    {
        var fakePlayerBucketService = Substitute.For<IPlayerBucketService>();

        var sut = new FillInPlayerGameTypeBucketEventHandler(
            NullLogger<FillInPlayerGameTypeBucketEventHandler>.Instance,
            fakePlayerBucketService);

        fakePlayerBucketService.GetBrandBucketTypesAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(new[] { BucketGameType.CardGame });

        var request = new FillInPlayerBucketEvent(new()
        {
            BrandId = Guid.NewGuid(),
            RoundId = Guid.NewGuid(),
            PlayerCode = "playerCode",
            Turnover = 100,
            EffectiveBettingPercentage = 100,
            GameType = GameType.CardGame
        }, DateTime.UtcNow);

        await sut.Handle(request, default);

        await fakePlayerBucketService.Received(1).FillInPlayerGameTypeBucketAsync(
            Arg.Is(request.PlayerRound.BrandId),
            Arg.Is(BucketGameType.CardGame),
            Arg.Is(request.PlayerRound.RoundId),
            Arg.Is(request.PlayerRound.PlayerCode),
            Arg.Is((decimal)100),
            Arg.Any<string>(),
            Arg.Any<CancellationToken>());
    }
}