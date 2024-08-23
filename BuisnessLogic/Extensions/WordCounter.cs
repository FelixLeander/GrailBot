using Discord.WebSocket;
using GrailBot.Data;
using GrailBot.Model;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace GrailBot.BuisnessLogic.Extensions;

public static class WordCounter
{
    public static async Task CountWords(SocketMessage socketMessage)
    {
        var groups = socketMessage.CleanContent.Split(new char[] { ' ', '\t', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries)
            .GroupBy(key => key)
            .ToList();


        foreach (var group in groups)
        {
            var dbContext = new DatabaseContext();
            var foundWord = dbContext.WordCounts.FirstOrDefault(wc => wc.Message.Equals(group.Key));

            if (foundWord != null)
            {
                foundWord.Amount += group.Count();
                Log.Verbose("Incremented word in db. Key:{groupKey} Amount:{amount}", group.Key, group.Count());
            }
            else
            {
                foundWord = new WordCount(group.Key)
                {
                    Amount = group.Count()
                };

                dbContext.WordCounts.Add(foundWord);
                Log.Verbose("Added new word to db. Key:{groupKey} Amount:{amount}", group.Key, group.Count());
            }

            try
            {
                await dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex.Message);
            }
        }
    }

    public static async Task WordLeaderbord(SocketMessage socketMessage)
    {
        var dbWordsQuery = new DatabaseContext().WordCounts.AsNoTracking();
        var topTen = dbWordsQuery.Take(10).OrderByDescending(wc => wc.Amount).ToList();
        var message = topTen == null || topTen.Count == 0
            ? "---Empty Board---"
            : $"Total Messages {dbWordsQuery.Count()}{Environment.NewLine}- {string.Join($"{Environment.NewLine}- ", topTen.Select(tt => $"{tt.Amount} | {tt.Message}"))}";

        Log.Verbose(message);

        await socketMessage.Channel.SendMessageAsync(message);
    }
}
