using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Tgs.Fill.In.Player.Bucket.Core;

namespace Tgs.Fill.In.Player.Bucket.Service.Controllers
{
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
        public Task FillInPlayerBucketAsync(DateTimeOffset startDate)
        {
            return _mediator.Send(new FillInPlayerBucketRequest(startDate));
        }
    }
}
