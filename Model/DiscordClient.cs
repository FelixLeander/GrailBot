using Discord.WebSocket;
using Discord;
using GrailBot.Discord;

namespace GrailBot.Model;

public class DiscordClient
{
    public DiscordSocketClient Client { get; set; }

    public DiscordClient()
    {
        Client = new DiscordSocketClient(new DiscordSocketConfig
        {
            GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.MessageContent //GatewayIntents.Guilds | GatewayIntents.GuildMessages
        });
    }

    public async Task Login(string token)
    {
        await Client.LoginAsync(TokenType.Bot, token);
        await Client.StartAsync();
    }

    public void SetupEventHandlers()
    {
        Client.MessageReceived += delegate (SocketMessage arg)
        {
            return new MessageReceived().Message_Received(arg);
        };
    }
}
