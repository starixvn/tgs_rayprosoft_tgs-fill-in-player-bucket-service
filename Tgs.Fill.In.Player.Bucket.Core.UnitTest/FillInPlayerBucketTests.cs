using MediatR;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using StackExchange.Redis;
using Tgs.Fill.In.Player.Bucket.Domain;
using Tgs.Fill.In.Player.Bucket.Domain.Events;
using Tgs.Fill.In.Player.Bucket.Domain.Models;
using Tgs.Fill.In.Player.Bucket.Redis;

namespace Tgs.Fill.In.Player.Bucket.Core.UnitTest;

public class FillInPlayerBucketTests
{
    [Test]
    public async Task No_Fill_In_Player_Bucket_When_No_Rounds()
    {
        var fakeTgsRepository = Substitute.For<ITgsRepository>();
        var fakeMediator = Substitute.For<IMediator>();
        var fakeRedis = Substitute.For<IRedisConnectionFactory>();
        var fakeRedisDatabase = Substitute.For<IDatabase>();

        var sut = new FillInPlayerBucketHandler(
            fakeTgsRepository,
            fakeMediator,
            fakeRedis,
            NullLogger<FillInPlayerBucketHandler>.Instance);

        fakeRedis.GetDataBase().Returns(fakeRedisDatabase);

        fakeRedisDatabase.SetContainsAsync(Arg.Any<RedisKey>(), Arg.Any<RedisValue>()).Returns(false);

        fakeTgsRepository.GetPlayerRoundsAsync(Arg.Any<DateTime>(), Arg.Any<DateTime>())
            .Returns(Array.Empty<PlayerRoundEntity>().ToAsyncEnumerable());

        var request = new FillInPlayerBucketRequest(DateTime.Parse("2023/08/01 00:00:00Z").ToUniversalTime());

        await sut.Handle(request, default);

        await fakeMediator.Received(0).Publish(Arg.Any<object>());
    }

    [Test]
    public async Task Fill_In_Player_Bucket()
    {
        var fakeTgsRepository = Substitute.For<ITgsRepository>();
        var fakeMediator = Substitute.For<IMediator>();
        var fakeRedis = Substitute.For<IRedisConnectionFactory>();
        var fakeRedisDatabase = Substitute.For<IDatabase>();

        var sut = new FillInPlayerBucketHandler(
            fakeTgsRepository,
            fakeMediator,
            fakeRedis,
            NullLogger<FillInPlayerBucketHandler>.Instance);

        fakeRedis.GetDataBase().Returns(fakeRedisDatabase);

        fakeRedisDatabase.SetContainsAsync(Arg.Any<RedisKey>(), Arg.Any<RedisValue>()).Returns(false);

        fakeTgsRepository.GetPlayerRoundsAsync(Arg.Any<DateTime>(), Arg.Any<DateTime>())
            .Returns(new PlayerRoundEntity[]
            {
                new()
                {
                    RoundId = Guid.NewGuid(),
                    PlayerCode = "playerCode"
                }
            }.ToAsyncEnumerable());

        var request = new FillInPlayerBucketRequest(DateTime.Parse("2023/08/01T00:00:00Z").ToUniversalTime());

        await sut.Handle(request, default);

        await fakeMediator.Received(1).Publish(Arg.Any<AddRoundRecordEvent>());
    }

    [Test]
    public async Task Fill_In_Player_Bucket_When_Redis_Record_Existed()
    {
        var fakeTgsRepository = Substitute.For<ITgsRepository>();
        var fakeMediator = Substitute.For<IMediator>();
        var fakeRedis = Substitute.For<IRedisConnectionFactory>();
        var fakeRedisDatabase = Substitute.For<IDatabase>();

        var sut = new FillInPlayerBucketHandler(
            fakeTgsRepository,
            fakeMediator,
            fakeRedis,
            NullLogger<FillInPlayerBucketHandler>.Instance);

        fakeRedis.GetDataBase().Returns(fakeRedisDatabase);

        fakeRedisDatabase.SetContainsAsync(Arg.Any<RedisKey>(), Arg.Any<RedisValue>()).Returns(true);

        fakeTgsRepository.GetPlayerRoundsAsync(Arg.Any<DateTime>(), Arg.Any<DateTime>())
            .Returns(Array.Empty<PlayerRoundEntity>().ToAsyncEnumerable());

        var request = new FillInPlayerBucketRequest(DateTime.Parse("2023/08/01 00:00:00Z").ToUniversalTime());

        await sut.Handle(request, default);

        await fakeMediator.Received(0).Publish(Arg.Any<object>());
    }

}