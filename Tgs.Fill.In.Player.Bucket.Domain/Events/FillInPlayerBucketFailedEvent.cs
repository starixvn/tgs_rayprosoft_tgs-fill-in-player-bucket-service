using MediatR;
using Tgs.Fill.In.Player.Bucket.Domain.Models;

namespace Tgs.Fill.In.Player.Bucket.Domain.Events;

public record FillInPlayerBucketFailedEvent(PlayerRoundEntity PlayerRound, DateTime StartDateTime) : INotification;