using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Tgs.Fill.In.Player.Bucket.Core.Events;
using Tgs.Fill.In.Player.Bucket.Domain.Events;
using Tgs.Fill.In.Player.Bucket.Domain.Interfaces;

namespace Tgs.Fill.In.Player.Bucket.Core.UnitTest;

public class FillInPlayerMainBucketEventTests
{
    [Test]
    public async Task Fill_In_Player_Main_Bucket()
    {
        var fakePlayerBucketService = Substitute.For<IPlayerBucketService>();

        var sut = new FillInPlayerMainBucketEventHandler(
            NullLogger<FillInPlayerMainBucketEventHandler>.Instance,
            fakePlayerBucketService);

        var request = new FillInPlayerBucketEvent(new()
        {
            RoundId = Guid.NewGuid(),
            PlayerCode = "playerCode",
            Turnover = 100,
            EffectiveBettingPercentage = 100
        }, DateTime.UtcNow);

        await sut.Handle(request, default);

        await fakePlayerBucketService.Received(1).FillInPlayerMainBucketAsync(
            Arg.Is(request.PlayerRound.RoundId),
            Arg.Is(request.PlayerRound.PlayerCode),
            Arg.Is((decimal)100),
            Arg.Any<string>(),
            Arg.Any<CancellationToken>());
    }
}