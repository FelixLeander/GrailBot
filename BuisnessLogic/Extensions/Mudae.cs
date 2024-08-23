using Discord;
using Discord.WebSocket;
using GrailBot.Model.Danbooru;
using RestSharp;
using Serilog;
using System.Diagnostics;

namespace GrailBot.BuisnessLogic.Extensions;

public static class Mudae
{
    public static async Task<bool> ClaimWaifus(object sender, SocketMessage socketMessage)
    {
        try
        {
            if (socketMessage.Author.Id != 432610292342587392)
                return false;

            var result = socketMessage.Embeds.First(e => e.Author.HasValue);
            if (result == null)
                return false;

            if (!(result.Author.HasValue && result.Author.Value.Name.Contains("Jibril", StringComparison.OrdinalIgnoreCase)))
                return false;

            var wait = Random.Shared.Next(0, 7) * 1000;
            await Task.Delay(wait);

            var react = socketMessage.Components.FirstOrDefault()?.Components.Cast<ButtonComponent>().FirstOrDefault();
            if (react == null)
            {
                Log.Warning("Invalid reaction {messageId}.", socketMessage.Id);
                return false;
            }

            await socketMessage.AddReactionAsync(react?.Emote);
            return true;
        }
        catch (Exception ex)
        {
            await socketMessage.Channel.SendMessageAsync($"Unexpected error:{Environment.NewLine}{ex.Message}");
        }

        return true;
    }
}
