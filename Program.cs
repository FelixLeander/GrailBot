using GrailBot.Database;
using GrailBot.Model;

namespace GrailBot;

public static class Program
{
    public static async Task<int> Main(string[] args)
    {
        try
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddEntityFrameworkSqlite().AddDbContext<DatabaseContext>();

            var botToken = builder.Configuration.GetValue<string>("BotConfig:Token");
            if (string.IsNullOrWhiteSpace(botToken))
                return -2;

            var discordClient = new DiscordClient();
            discordClient.Client.Log += Client_Log;
            await discordClient.Login(botToken);
            discordClient.SetupEventHandlers();
            builder.Services.AddSingleton<DiscordClient>();

            using (var client = new DatabaseContext())
            {
                client.Database.EnsureCreated();
            }

            var app = builder.Build();
            app.Run();

            Console.WriteLine("Exiting application.");
            return 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Unexpected error:");
            Console.WriteLine(ex.Message);
            Console.WriteLine(ex.StackTrace);
            return -1;
        }
    }

    private static Task Client_Log(global::Discord.LogMessage arg)
    {
        Console.WriteLine(arg.ToString());
        return Task.CompletedTask;
    }
}