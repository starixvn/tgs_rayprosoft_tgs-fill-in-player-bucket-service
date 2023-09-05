using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using StackExchange.Redis;
using Tgs.Fill.In.Player.Bucket.Client;
using Tgs.Fill.In.Player.Bucket.Domain;
using Tgs.Fill.In.Player.Bucket.Domain.Interfaces;
using Tgs.Fill.In.Player.Bucket.Domain.Models;
using Tgs.Fill.In.Player.Bucket.Redis;

namespace Tgs.Fill.In.Player.Bucket.IntegrationTest;

[TestFixture]
public class PlayerBucketTest
{
    [Test]
    public async Task Fill_In_Player_bucket()
    {
        var fakeTgsRepository = Substitute.For<ITgsRepository>();
        var fakeRedisConnectionFactory = Substitute.For<IRedisConnectionFactory>();
        var fakeRedisDatabase = Substitute.For<IDatabase>();
        var fakePlayerBucketService = Substitute.For<IPlayerBucketService>();

        var roundId = Guid.NewGuid();

        var rounds = new PlayerRoundEntity[]
        {
            new()
            {
                RoundId = roundId,
                GameId = 1,
                PlayerCode = "playerCode",
                ExternalRoundId = "external1",
                Turnover = 100,
                EffectiveBettingPercentage = 100,
                GameType = GameType.CardGame
            }
        };
        {
            fakeTgsRepository.GetPlayerRoundsAsync(Arg.Any<DateTime>(), Arg.Any<DateTime>())
                .Returns(rounds.ToAsyncEnumerable());

            fakePlayerBucketService.GetBrandBucketTypesAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
                .Returns(new[] { BucketGameType.CardGame });

            fakeRedisConnectionFactory.GetDataBase()
                .Returns(fakeRedisDatabase);

            fakeRedisDatabase.SetContainsAsync(Arg.Any<RedisKey>(), Arg.Any<RedisValue>()).Returns(false);
        }

        await using var web = new TestApplication(builder =>
            builder.ConfigureServices(service =>
                {
                    service.AddSingleton(fakeTgsRepository);
                    service.AddSingleton(fakeRedisConnectionFactory);
                    service.AddSingleton(fakeRedisDatabase);
                    service.AddSingleton(fakePlayerBucketService);
                }
            ));

        var httpClient = web.CreateClient();
        {
            var sut = new FillInPlayerBucketClient(httpClient);

            var now = DateTime.Parse("2023-08-01 00:15:00Z");

            await sut.FillPlayerBucketMinutelyAsync(new(now));

            await fakeRedisDatabase.Received(1).SetAddAsync(
                Arg.Is((RedisKey)rounds[0].GetRedisKey(now)),
                Arg.Is((RedisValue)"external1"));

            await fakeRedisDatabase.Received(1).KeyExpireAsync(
                Arg.Is((RedisKey)rounds[0].GetRedisKey(now)),
                Arg.Is(TimeSpan.FromDays(49)));

            await fakePlayerBucketService.Received(1).FillInPlayerGameTypeBucketAsync(
                Arg.Any<BucketGameType>(),
                Arg.Is(roundId),
                Arg.Is("playerCode"),
                Arg.Is((decimal)100),
                Arg.Any<string>(),
                Arg.Any<CancellationToken>());
        }
    }

    [Test]
    public async Task No_Fill_In_Player_bucket_When_Filled()
    {
        var fakeTgsRepository = Substitute.For<ITgsRepository>();
        var fakeRedisConnectionFactory = Substitute.For<IRedisConnectionFactory>();
        var fakeRedisDatabase = Substitute.For<IDatabase>();
        var fakePlayerBucketService = Substitute.For<IPlayerBucketService>();

        var roundId = Guid.NewGuid();
        {
            fakeTgsRepository.GetPlayerRoundsAsync(Arg.Any<DateTime>(), Arg.Any<DateTime>())
                .Returns(new PlayerRoundEntity[]
                {
                    new()
                    {
                        RoundId = roundId,
                        GameId = 1,
                        PlayerCode = "playerCode",
                        ExternalRoundId = "external1",
                        Turnover = 100,
                        EffectiveBettingPercentage = 100,
                        GameType = GameType.Sportsbook
                    }
                }.ToAsyncEnumerable());

            fakeRedisConnectionFactory.GetDataBase()
                .Returns(fakeRedisDatabase);

            fakeRedisDatabase.SetContainsAsync(Arg.Any<RedisKey>(), Arg.Any<RedisValue>()).Returns(true);
        }

        await using var web = new TestApplication(builder =>
            builder.ConfigureServices(service =>
                {
                    service.AddSingleton(fakeTgsRepository);
                    service.AddSingleton(fakeRedisConnectionFactory);
                    service.AddSingleton(fakeRedisDatabase);
                    service.AddSingleton(fakePlayerBucketService);
                }
            ));

        var httpClient = web.CreateClient();
        {
            var sut = new FillInPlayerBucketClient(httpClient);

            await sut.FillPlayerBucketMinutelyAsync(new(DateTime.Parse("2023-08-01 00:15:00Z")));

            await fakeRedisDatabase.Received(1).SetContainsAsync(
                Arg.Any<RedisKey>(),
                Arg.Any<RedisValue>());

            await fakeRedisDatabase.Received(0).SetAddAsync(
                Arg.Any<RedisKey>(),
                Arg.Any<RedisValue>());

            await fakeRedisDatabase.Received(0).KeyExpireAsync(
                Arg.Any<RedisKey>(),
                Arg.Any<TimeSpan>());

            _ = fakeTgsRepository.Received(0).GetPlayerRoundsAsync(
                Arg.Any<DateTime>(),
                Arg.Any<DateTime>());

            await fakePlayerBucketService.Received(0).FillInPlayerGameTypeBucketAsync(
                Arg.Any<BucketGameType>(),
                Arg.Any<Guid>(),
                Arg.Any<string>(),
                Arg.Any<decimal>(),
                Arg.Any<string>(),
                Arg.Any<CancellationToken>());
        }
    }

    [Test]
    public async Task No_Fill_In_Player_bucket_When_No_Rounds()
    {
        var fakeTgsRepository = Substitute.For<ITgsRepository>();
        var fakeRedisConnectionFactory = Substitute.For<IRedisConnectionFactory>();
        var fakeRedisDatabase = Substitute.For<IDatabase>();
        var fakePlayerBucketService = Substitute.For<IPlayerBucketService>();

        {
            fakeTgsRepository.GetPlayerRoundsAsync(Arg.Any<DateTime>(), Arg.Any<DateTime>())
                .Returns(Array.Empty<PlayerRoundEntity>().ToAsyncEnumerable());

            fakeRedisConnectionFactory.GetDataBase()
                .Returns(fakeRedisDatabase);

            fakeRedisDatabase.SetContainsAsync(Arg.Any<RedisKey>(), Arg.Any<RedisValue>()).Returns(false);
        }

        await using var web = new TestApplication(builder =>
            builder.ConfigureServices(service =>
                {
                    service.AddSingleton(fakeTgsRepository);
                    service.AddSingleton(fakeRedisConnectionFactory);
                    service.AddSingleton(fakeRedisDatabase);
                    service.AddSingleton(fakePlayerBucketService);
                }
            ));

        var httpClient = web.CreateClient();
        {
            var sut = new FillInPlayerBucketClient(httpClient);

            await sut.FillPlayerBucketMinutelyAsync(new(DateTime.Parse("2023-08-01 00:15:00Z")));

            await fakeRedisDatabase.Received(1).SetContainsAsync(
                Arg.Any<RedisKey>(),
                Arg.Any<RedisValue>());

            await fakeRedisDatabase.Received(1).SetAddAsync(
                Arg.Any<RedisKey>(),
                Arg.Any<RedisValue>());

            await fakeRedisDatabase.Received(1).KeyExpireAsync(
                Arg.Any<RedisKey>(),
                Arg.Any<TimeSpan>());

            _ = fakeTgsRepository.Received(1).GetPlayerRoundsAsync(
                Arg.Any<DateTime>(),
                Arg.Any<DateTime>());

            await fakePlayerBucketService.Received(0).FillInPlayerGameTypeBucketAsync(
                Arg.Any<BucketGameType>(),
                Arg.Any<Guid>(),
                Arg.Any<string>(),
                Arg.Any<decimal>(),
                Arg.Any<string>(),
                Arg.Any<CancellationToken>());
        }
    }
}