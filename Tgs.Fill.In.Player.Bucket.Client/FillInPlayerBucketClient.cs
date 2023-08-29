using System.Text;
using System.Text.Json;
using Tgs.Fill.In.Player.Bucket.Client.Models;

namespace Tgs.Fill.In.Player.Bucket.Client;

public class FillInPlayerBucketClient : IFillInPlayerBucketClient
{
    private readonly HttpClient _client;

    private readonly JsonSerializerOptions _jsonSerializerOptions = new()
        { PropertyNameCaseInsensitive = true };

    public FillInPlayerBucketClient(HttpClient client)
    {
        _client = client;
    }

    public async Task FillPlayerBucketMinutelyAsync(
        FillPlayerBucketMinutelyModel request)
    {
        var httpRequest = new HttpRequestMessage(HttpMethod.Post, $"/api/playerBucket/fillIn/{request.StartDate:u}")
        {
            Content = new StringContent("", Encoding.UTF8, "application/json")
        };
        var response = await _client.SendAsync(httpRequest).ConfigureAwait(false);

        _ = response.EnsureSuccessStatusCode();
    }
}