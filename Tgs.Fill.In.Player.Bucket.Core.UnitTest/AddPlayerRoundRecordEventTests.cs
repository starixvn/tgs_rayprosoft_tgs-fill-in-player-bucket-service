using MediatR;
using NSubstitute;
using StackExchange.Redis;
using Tgs.Fill.In.Player.Bucket.Core.Events;
using Tgs.Fill.In.Player.Bucket.Domain.Events;
using Tgs.Fill.In.Player.Bucket.Redis;

namespace Tgs.Fill.In.Player.Bucket.Core.UnitTest;

public class AddPlayerRoundRecordEventTests
{
    [Test]
    public async Task Can_Not_Add_Player_Round_Record_Event_When_Key_Value_Existed()
    {
        var fakeRedisConnectionFactory = Substitute.For<IRedisConnectionFactory>();
        var fakeRedisDatabase = Substitute.For<IDatabase>();
        var fakeMediator = Substitute.For<IMediator>();

        var sut = new AddPlayerRoundRecordEventHandler(
            fakeRedisConnectionFactory,
            fakeMediator);

        fakeRedisConnectionFactory.GetDataBase()
            .Returns(fakeRedisDatabase);

        fakeRedisDatabase.SetContainsAsync(Arg.Any<RedisKey>(), Arg.Any<RedisValue>()).Returns(true);

        var request = new AddRoundRecordEvent(new()
        {
            GameId = 1,
            PlayerCode = "playerCode"
        }, DateTime.UtcNow);

        await sut.Handle(request, default);

        await fakeRedisDatabase.Received(0).SetAddAsync(Arg.Any<RedisKey>(), Arg.Any<RedisValue>());

        await fakeRedisDatabase.Received(0).KeyExpireAsync(Arg.Any<RedisKey>(), Arg.Any<TimeSpan>());

        await fakeMediator.Received(0).Publish(Arg.Any<FillInPlayerBucketEvent>());
    }

    [Test]
    public async Task Add_Player_Round_Record_Event()
    {
        var fakeRedisConnectionFactory = Substitute.For<IRedisConnectionFactory>();
        var fakeRedisDatabase = Substitute.For<IDatabase>();
        var fakeMediator = Substitute.For<IMediator>();

        var sut = new AddPlayerRoundRecordEventHandler(
            fakeRedisConnectionFactory,
            fakeMediator);

        fakeRedisConnectionFactory.GetDataBase()
            .Returns(fakeRedisDatabase);

        fakeRedisDatabase.SetContainsAsync(Arg.Any<RedisKey>(), Arg.Any<RedisValue>()).Returns(false);

        var now = DateTime.Parse("2023/08/28 00:00:00Z");

        var request = new AddRoundRecordEvent(new()
        {
            GameId = 1,
            PlayerCode = "playerCode",
            ExternalRoundId = "external1"
        }, now);

        await sut.Handle(request, default);

        await fakeRedisDatabase.Received(1).SetAddAsync(Arg.Is((RedisKey)request.PlayerRound.GetRedisKey(now)),
            Arg.Is((RedisValue)"external1"));

        await fakeRedisDatabase.Received(1).KeyExpireAsync(Arg.Is((RedisKey)request.PlayerRound.GetRedisKey(now)),
            Arg.Is(TimeSpan.FromDays(49)));

        await fakeMediator.Received(1).Publish(Arg.Any<FillInPlayerBucketEvent>());
    }
}