using Tgs.Fill.In.Player.Bucket.Client.Models;

namespace Tgs.Fill.In.Player.Bucket.Client
{
	public interface IFillInPlayerBucketClient
	{
		Task FillPlayerBucketMinutelyAsync(FillPlayerBucketMinutelyModel request);
	}
}
