using MediatR;
using Tgs.Fill.In.Player.Bucket.Domain.Models;

namespace Tgs.Fill.In.Player.Bucket.Domain.Events
{
    public record FillInPlayerBucketEvent(PlayerRoundEntity PlayerRound, DateTime StartDateTime) : INotification;
}
