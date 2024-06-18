using MediatR;
using Microsoft.AspNetCore.Mvc;
using Tgs.Fill.In.Player.Bucket.Core;

namespace Tgs.Fill.In.Player.Bucket.Service.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PlayerBucketController : ControllerBase
{
    private readonly IMediator _mediator;

    public PlayerBucketController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("fillIn/{startDate}")]
    public Task FillInPlayerBucketAsync(DateTime startDate) => _mediator.Send(new FillInPlayerBucketRequest(startDate));

    [HttpGet("fill-in-failed-records")]
    public Task FillInPlayerBucketFailedRecordsAsync()
    {
        return _mediator.Send(new FillInPlayerBucketFromFailedRecordRequest());
    }

    [HttpGet("2hours/{startDate}")]
    public void FillInPlayerBucket2Hours(DateTime startDate)
    {
        var startTime = startDate; // 開始時間
        var endTime = startTime.AddHours(2);                                      // 結束時間，這裡設定為開始時間後2小時

        var timeArray = GenerateTimeArray(startTime, endTime, TimeSpan.FromMinutes(1));

        timeArray.ForEach(x => _mediator.Send(new FillInPlayerBucketRequest(x)));
    }

    private static List<DateTime> GenerateTimeArray(DateTime start, DateTime end, TimeSpan interval)
    {
        var timeArray = new List<DateTime>();
        var currentTime = start;

        while (currentTime <= end)
        {
            timeArray.Add(currentTime);
            currentTime = currentTime.Add(interval);
        }

        return timeArray;
    }
}