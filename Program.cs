using Discord;
using Discord.WebSocket;
using GrailBot.BuisnessLogic;
using GrailBot.Data;
using Serilog;
using Serilog.Events;

namespace GrailBot;

public static class Program
{
    public static async Task<int> Main(string[] args)
    {
        DiscordManager? discordManager = null;
        try
        {
            var filePath = Path.Combine("logs", "log.log");
            var dirPath = Directory.GetParent(filePath)?.FullName ?? "";
            if (dirPath != null)
                Directory.CreateDirectory(dirPath);

            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console(restrictedToMinimumLevel: LogEventLevel.Verbose)
                .WriteTo.File(filePath, LogEventLevel.Warning, rollingInterval: RollingInterval.Month)
                .CreateLogger();

            Log.Information("Starting...");

            string executableFilePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string? executableParentDir = Path.GetDirectoryName(executableFilePath);
            if (executableParentDir != null)
                Environment.CurrentDirectory = executableParentDir;
            await Console.Out.WriteLineAsync($"WorkingDir: {Environment.CurrentDirectory}");


            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddEntityFrameworkSqlite().AddDbContext<DatabaseContext>();

            var botToken = builder.Configuration.GetValue<string>("BotConfig:Token");
            if (string.IsNullOrWhiteSpace(botToken))
                return -2;

            using (var client = new DatabaseContext())
            {
                client.Database.EnsureCreated();
            }

            discordManager = new DiscordManager();
            discordManager.Client.Log += HandleLogMessage;
            discordManager.Client.MessageReceived += new DiscordManager().Message_Send;

            await discordManager.Login(botToken);

            builder.Services.AddSingleton(discordManager);

            var app = builder.Build();
            app.Run();

            Log.Verbose("Exiting application.");
            return 0;
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Unexpected error.");
            return -1;
        }
        finally
        {
            Console.WriteLine("Finally:");
            try
            {
                if (discordManager != null)
                    await discordManager.Client.LogoutAsync();
                Log.Verbose("Logged-off discord on exit.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Could not log-off bot on exit.");
            }

            Log.Verbose("Flushing Logger");
            Log.CloseAndFlush();
        }
    }

    private static Task HandleLogMessage(LogMessage logMessage)
    {
        switch (logMessage.Severity)
        {
            case LogSeverity.Verbose:
                Log.Verbose(logMessage.Exception, logMessage.Message);
                break;

            case LogSeverity.Debug:
                Log.Debug(logMessage.Exception, logMessage.Message);
                break;

            case LogSeverity.Info:
                Log.Information(logMessage.Exception, logMessage.Message);
                break;

            case LogSeverity.Warning:
                Log.Warning(logMessage.Exception, logMessage.Message);
                break;

            case LogSeverity.Error:
                Log.Error(logMessage.Exception, logMessage.Message);
                break;

            case LogSeverity.Critical:
                Log.Fatal(logMessage.Exception, logMessage.Message);
                break;

            default:
                Log.Fatal(logMessage.Exception, logMessage.Message);
                break;
        }
        return Task.CompletedTask;
    }
}