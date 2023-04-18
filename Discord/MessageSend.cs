using Discord.WebSocket;
using GrailBot.Database;
using GrailBot.Model;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace GrailBot.Discord;

public class MessageSend
{
    public async Task Called(SocketMessage socketMessage)
    {
        await Console.Out.WriteLineAsync(string.IsNullOrEmpty(socketMessage.Content) ? "Empty-Message" : socketMessage.Content);

        if (socketMessage.Author.IsBot)
            return;

        if (socketMessage.CleanContent.StartsWith("<3LeaderBoard"))
        {
            await WordLeaderbord(socketMessage);
            return;
        }

        var messageParts = socketMessage.CleanContent.Split(new char[] { ' ', '\t', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries).ToList();

        foreach (var word in messageParts)
        {
            var dbContext = new DatabaseContext();
            var foundWord = dbContext.WordCounts.FirstOrDefault(wc => wc.Message.Equals(word));

            if (foundWord != null)
                foundWord.Amount++;
            else
                dbContext.WordCounts.Add(new WordCount(word));

            dbContext.SaveChanges();
        };
    }

    public async Task WordLeaderbord(SocketMessage socketMessage)
    {
        var topTen = new DatabaseContext().WordCounts.AsNoTracking().Take(10).OrderBy(wc => wc.Amount).Select(wc => $"{wc.Amount} {wc.Message}").ToList();
        var message = topTen == null || topTen.Count == 0 ? "---Empty Board---" : string.Join(Environment.NewLine, topTen);
        await socketMessage.Channel.SendMessageAsync(message);
    }
}
