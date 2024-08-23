using Discord.Rest;
using Discord.WebSocket;

namespace GrailBot.BuisnessLogic;

public static class Helper
{
    private const string CommandPrefix = "!!";

    internal static bool GetMatch(this SocketMessage socketMessage, string slimCommand, out string fullCommand, out string commandLine)
    {
        var combined = $"{CommandPrefix}{slimCommand}";

        var lines = socketMessage.Content.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        var line = lines.FirstOrDefault(f => f.StartsWith(combined, StringComparison.OrdinalIgnoreCase));

        fullCommand = combined;
        commandLine = line ?? string.Empty;
        return line != null;
    }

    internal static bool IsCommandCallWith(this SocketMessage socketMessage, string commandName, out string fullCommand)
    {
        fullCommand = $"{CommandPrefix}{commandName}";
        return socketMessage.CleanContent.StartsWith(fullCommand, StringComparison.OrdinalIgnoreCase);
    }

    internal static bool IsCommandCallWith(this SocketMessage socketMessage, string commandName)
        => socketMessage.CleanContent.StartsWith($"{CommandPrefix}{commandName}", StringComparison.OrdinalIgnoreCase);
}
