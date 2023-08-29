using MediatR;
using Tgs.Fill.In.Player.Bucket.Domain.Models;

namespace Tgs.Fill.In.Player.Bucket.Domain.Events;

public record AddRoundRecordEvent(PlayerRoundEntity PlayerRound, DateTime StartDateTime) : INotification;