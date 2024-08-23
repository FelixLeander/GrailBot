using Discord.WebSocket;
using GrailBot.Model.Danbooru;
using RestSharp;
using System.Text.Json;

namespace GrailBot.BuisnessLogic.Extensions;

public static class Danbooru
{
    private const string DomainAdress = @"https://danbooru.donmai.us";
    private const string ApiKey = @"vy25T3FteHYP3Re33UdQ69Nw";
    private static RestRequest GetLoginRequest(string resource)
                    => new RestRequest(resource)
                        .AddQueryParameter("login", "Akayaaa")
                        .AddQueryParameter("api_key", ApiKey);

    internal static async Task<bool> GetSpecific(object sender, SocketMessage socketMessage)
    {
        try
        {
            if (!socketMessage.GetMatch("post", out string fullCommand, out string commandLine))
                return false;

            var postId = commandLine.Split(' ', StringSplitOptions.TrimEntries).LastOrDefault();

            var restClient = new RestClient(DomainAdress);
            var request = GetLoginRequest($"/posts/{postId}.json");

            var restResult = restClient.Execute(request);
            if (restResult.Content == null)
            {
                await socketMessage.Channel.SendMessageAsync("Invalid Server response");
                return true;
            }

            var result = JsonSerializer.Deserialize<PostResponse>(restResult.Content);
            if (result == null)
            {
                await socketMessage.Channel.SendMessageAsync($"Parsing error.");
                return true;
            }

            await socketMessage.Channel.SendMessageAsync(result.FileUrl);
        }
        catch (Exception ex)
        {
            await socketMessage.Channel.SendMessageAsync($"Unexpected error:{Environment.NewLine}{ex.Message}");
        }

        return true;
    }

    internal static async Task<bool> GetRandomPostByTags(object sender, SocketMessage socketMessage)
    {
        try
        {
            if (!socketMessage.GetMatch("fromTag", out string fullCommand, out string commandLine))
                return false;

            var body = commandLine[fullCommand.Length..];
            var uncleanTags = body.Split(',', StringSplitOptions.RemoveEmptyEntries);
            var addUnderscored = uncleanTags.Select(s => s.Trim().Replace(' ', '_')).ToArray();
            var parameterTags = string.Join('+', addUnderscored);



            var restClient = new RestClient(DomainAdress);
            var request = GetLoginRequest("/posts.json")
                         .AddQueryParameter("tags", parameterTags, false);

            var restResult = restClient.Execute(request);
            if (restResult.Content == null)
            {
                await socketMessage.Channel.SendMessageAsync($"You used an invalid tag, used tags:{Environment.NewLine}{parameterTags}");
                return true;
            }

            var result = JsonSerializer.Deserialize<List<PostResponse>>(restResult.Content);
            if (result == null)
            {
                await socketMessage.Channel.SendMessageAsync($"Parsing error.");
                return true;
            }

            var rngIndex = Random.Shared.Next(0, result.Count);
            var randomResult = result[rngIndex];
            await socketMessage.Channel.SendMessageAsync(randomResult.FileUrl);
        }
        catch (Exception ex)
        {
            await socketMessage.Channel.SendMessageAsync($"Unexpected error:{Environment.NewLine}{ex.Message}");
        }

        return true;
    }
}
