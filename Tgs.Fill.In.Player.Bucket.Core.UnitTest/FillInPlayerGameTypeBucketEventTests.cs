using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Tgs.Fill.In.Player.Bucket.Core.Events;
using Tgs.Fill.In.Player.Bucket.Domain;
using Tgs.Fill.In.Player.Bucket.Domain.Events;
using Tgs.Fill.In.Player.Bucket.Domain.Interfaces;
using Tgs.Fill.In.Player.Bucket.Domain.Models;

namespace Tgs.Fill.In.Player.Bucket.Core.UnitTest
{
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

            var request = new FillInPlayerBucketEvent(new PlayerRoundEntity
            {
                RoundId = Guid.NewGuid(),
                PlayerCode = "playerCode",
                Turnover = 100,
                EffectiveBettingPercentage = 100,
                GameType = GameType.CardGame,
            }, DateTime.UtcNow);

            await sut.Handle(request, default);

            await fakePlayerBucketService.Received(1).FillInPlayerGameTypeBucketAsync(
                Arg.Any<IEnumerable<string>>(),
                Arg.Is(request.PlayerRound.RoundId),
                Arg.Is(request.PlayerRound.PlayerCode),
                Arg.Is((decimal)100),
                Arg.Any<string>(),
                Arg.Any<CancellationToken>());
        }

        [Test]
        public async Task No_Fill_In_Player_Game_Type_Bucket_When_No_Match_Types()
        {
            var fakePlayerBucketService = Substitute.For<IPlayerBucketService>();

            var sut = new FillInPlayerGameTypeBucketEventHandler(
                NullLogger<FillInPlayerGameTypeBucketEventHandler>.Instance,
                fakePlayerBucketService);

            fakePlayerBucketService.GetBrandBucketTypesAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
                .Returns(new[] { BucketGameType.CardGame });

            var request = new FillInPlayerBucketEvent(new PlayerRoundEntity
            {
                RoundId = Guid.NewGuid(),
                PlayerCode = "playerCode",
                Turnover = 100,
                EffectiveBettingPercentage = 100,
                GameType = GameType.Sportsbook,
            }, DateTime.UtcNow);

            await sut.Handle(request, default);

            await fakePlayerBucketService.Received(0).FillInPlayerGameTypeBucketAsync(
                Arg.Any<IEnumerable<string>>(),
                Arg.Any<Guid>(),
                Arg.Any<string>(),
                Arg.Is((decimal)100),
                Arg.Any<string>(),
                Arg.Any<CancellationToken>());
        }
    }
}