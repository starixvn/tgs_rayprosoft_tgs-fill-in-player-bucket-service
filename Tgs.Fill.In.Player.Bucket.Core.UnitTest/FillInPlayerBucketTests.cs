using MediatR;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Tgs.Fill.In.Player.Bucket.Domain;
using Tgs.Fill.In.Player.Bucket.Domain.Events;
using Tgs.Fill.In.Player.Bucket.Domain.Models;

namespace Tgs.Fill.In.Player.Bucket.Core.UnitTest;

public class FillInPlayerBucketTests
{
    [Test]
    public async Task Fill_In_Player_Bucket_When_No_Rounds()
    {
        var fakeTgsRepository = Substitute.For<ITgsRepository>();
        var fakeMediator = Substitute.For<IMediator>();

        var sut = new FillInPlayerBucketHandler(
            fakeTgsRepository,
            fakeMediator,
            NullLogger<FillInPlayerBucketHandler>.Instance);

        fakeTgsRepository.GetPlayerRoundsAsync(Arg.Any<DateTime>(), Arg.Any<DateTime>())
            .Returns(Array.Empty<PlayerRoundEntity>().ToAsyncEnumerable());

        var request = new FillInPlayerBucketRequest(DateTime.Parse("2023/08/01 00:00:00Z"));

        await sut.Handle(request, default);

        await fakeMediator.Received(0).Publish(Arg.Any<object>());
    }

    [Test]
    public async Task Fill_In_Player_Bucket()
    {
        var fakeTgsRepository = Substitute.For<ITgsRepository>();
        var fakeMediator = Substitute.For<IMediator>();

        var sut = new FillInPlayerBucketHandler(
            fakeTgsRepository,
            fakeMediator,
            NullLogger<FillInPlayerBucketHandler>.Instance);

        fakeTgsRepository.GetPlayerRoundsAsync(Arg.Any<DateTime>(), Arg.Any<DateTime>())
            .Returns(new PlayerRoundEntity[]
            {
                new()
                {
                    RoundId = Guid.NewGuid(),
                    PlayerCode = "playerCode"
                }
            }.ToAsyncEnumerable());

        var request = new FillInPlayerBucketRequest(DateTime.Parse("2023/08/01 00:00:00Z"));

        await sut.Handle(request, default);

        await fakeMediator.Received(1).Publish(Arg.Any<AddRoundRecordEvent>());
    }
}