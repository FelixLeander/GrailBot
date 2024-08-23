using Discord;
using Discord.Commands;
using Discord.WebSocket;
using GrailBot.BuisnessLogic.Extensions;
using GrailBot.Data;
using GrailBot.Model;
using GrailBot.Model.Danbooru;
using Microsoft.EntityFrameworkCore;
using RestSharp;
using Serilog;
using System.Text.Json;

namespace GrailBot.BuisnessLogic;

public class DiscordManager : ModuleBase<SocketCommandContext>
{
    public DiscordSocketClient Client { get; set; } = new DiscordSocketClient(new DiscordSocketConfig
    {
        GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.MessageContent //GatewayIntents.Guilds | GatewayIntents.GuildMessages
    });
    private event DiscordCommand UserCommandReceived;
    private event DiscordCommand BotCommandReceived;
    private delegate Task<bool> DiscordCommand(object sender, SocketMessage socketMessage);

    public DiscordManager()
    {
        UserCommandReceived += Danbooru.GetRandomPostByTags;
        UserCommandReceived += Danbooru.GetSpecific;
        BotCommandReceived += Mudae.ClaimWaifus;

        Client.ButtonExecuted += Client_ButtonExecuted;
        Client.MessageUpdated += Client_MessageUpdated;
    }

    private async Task Client_MessageUpdated(Cacheable<IMessage, ulong> arg1, SocketMessage arg2, ISocketMessageChannel arg3)
    {
        var result = arg2.Components.FirstOrDefault()?.Components.FirstOrDefault();
        var button = result as ButtonComponent;

        var daada = button?.ToBuilder().Build();

        var aaa = new ComponentBuilder().WithButton().Build();

        await ReplyAsync(components: aaa);
    }

    private async Task Client_ButtonExecuted(SocketMessageComponent arg)
    {
        var result = arg.Data.CustomId;
        await arg.RespondAsync("SUCK IT!");
        await arg.Channel.SendMessageAsync(result);
    }

    public async Task Login(string token)
    {
        await Client.LoginAsync(TokenType.Bot, token);
        await Client.StartAsync();
    }

    public async Task Message_Send(SocketMessage socketMessage)
    {
        Log.Verbose(string.IsNullOrEmpty(socketMessage.Content) ? "Empty-Message" : socketMessage.Content);

        if (BotCommandReceived != null && socketMessage.Author.IsBot)
            await BotCommandReceived.Invoke(Client, socketMessage);
        else
            await UserCommandReceived.Invoke(Client, socketMessage);


        //if (socketMessage.Channel.Id == 1167147203396128768)
        //{
        //    await socketMessage.Channel.SendMessageAsync(socketMessage.Content);
        //    return;
        //}

        if (socketMessage.CleanContent.StartsWith($"!!lb"))
        {
            await WordCounter.WordLeaderbord(socketMessage);
            return;
        }

        if (!socketMessage.CleanContent.StartsWith($"!!"))
            await WordCounter.CountWords(socketMessage);
    }
}
