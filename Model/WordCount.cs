using System.ComponentModel.DataAnnotations;

namespace GrailBot.Model;

public class WordCount
{
    [Key]
    public int Id { get; set; }
    public string Message { get; set; }
    public int Amount { get; set; }

    public WordCount(string message)
    {
        Message = message;
    }
}
