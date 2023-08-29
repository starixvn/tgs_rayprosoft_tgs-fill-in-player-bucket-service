using MediatR;

namespace Tgs.Fill.In.Player.Bucket.Core;

public record FillInPlayerBucketRequest(DateTime StartDateTime) : IRequest;